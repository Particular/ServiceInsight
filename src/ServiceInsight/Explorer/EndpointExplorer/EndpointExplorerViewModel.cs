namespace ServiceInsight.Explorer.EndpointExplorer
{
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Caliburn.Micro;
    using Framework.UI;
    using ServiceInsight.ExtensionMethods;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.Settings;
    using ServiceInsight.ServiceControl;
    using ServiceInsight.Settings;
    using ServiceInsight.Shell;
    using ServiceInsight.Startup;
    using ServiceInsight.MessageList;

    public class EndpointExplorerViewModel : Screen,
        IHandle<RequestSelectingEndpoint>,
        IHandleWithTask<ConfigurationUpdated>
    {
        readonly IEventAggregator eventAggregator;
        readonly IWorkNotifier workNotifier;
        readonly ISettingsProvider settingsProvider;
        readonly CommandLineArgParser commandLineParser;
        readonly ServiceControlClientRegistry clientRegistry;
        readonly MessageSelectionContext messageSelectionContext;
        IEndpointExplorerView view;
        bool isStartingUp;

        public EndpointExplorerViewModel(
            IEventAggregator eventAggregator,
            IWorkNotifier workNotifier,
            ISettingsProvider settingsProvider,
            CommandLineArgParser commandLineParser,
            ServiceControlClientRegistry clientRegistry,
            MessageSelectionContext messageSelectionContext)
        {
            this.eventAggregator = eventAggregator;
            this.workNotifier = workNotifier;
            this.settingsProvider = settingsProvider;
            this.clientRegistry = clientRegistry;
            this.messageSelectionContext = messageSelectionContext;
            this.commandLineParser = commandLineParser;
            Items = new BindableCollection<ExplorerItem>();
        }

        public IObservableCollection<ExplorerItem> Items { get; }

        ExplorerItem _selectedNode;
        public ExplorerItem SelectedNode
        {
            get
            {
                return _selectedNode;
            }
            set
            {
                if (_selectedNode?.DisplayMemberPath != value?.DisplayMemberPath)
                {
                    _selectedNode = value;
                    NotifyOfPropertyChange(() => SelectedNode);
                }
            }
        }

        public new ShellViewModel Parent => (ShellViewModel)base.Parent;

        bool IsConnected => HasConnectedToAnyServiceControl;

        public bool HasSelectedServiceControl => GetSelectedServiceControl() != null;

        public bool HasConnectedToAnyServiceControl => ServiceControls.Any();

        public ServiceControlExplorerItem GetSelectedServiceControl()
        {
            var selected = SelectedNode;
            if (selected != null)
            {
                if (selected is ServiceControlExplorerItem item)
                {
                    return item;
                }

                if (selected is EndpointExplorerItem)
                {
                    var endpoint = (AuditEndpointExplorerItem)selected;
                    return endpoint.ServiceControl;
                }
            }
            else
            {
                return ServiceControls.FirstOrDefault();
            }

            return null;
        }

        public IList<ServiceControlExplorerItem> ServiceControls => Items.OfType<ServiceControlExplorerItem>().ToList();

        protected override async void OnActivate()
        {
            base.OnActivate();

            try
            {
                isStartingUp = true;

                if (IsConnected)
                {
                    return;
                }

                var url = GetConfiguredAddress();
                await ConnectToService(url);
            }
            finally
            {
                isStartingUp = false;
            }
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            this.view = (IEndpointExplorerView)view;
        }

        string GetConfiguredAddress()
        {
            if (commandLineParser.ParsedOptions.EndpointUri == null)
            {
                var appSettings = settingsProvider.GetSettings<ProfilerSettings>();
                if (appSettings != null && appSettings.LastUsedServiceControl != null)
                {
                    return appSettings.LastUsedServiceControl;
                }

                var managementConfig = settingsProvider.GetSettings<ServiceControlSettings>();
                return string.Format("http://localhost:{0}/api", managementConfig.Port);
            }

            return commandLineParser.ParsedOptions.EndpointUri.ToString();
        }

        ServiceControlExplorerItem AddServiceNode(string url)
        {
            var serviceControlNode = new ServiceControlExplorerItem(url);
            Items.Add(serviceControlNode);
            return serviceControlNode;
        }

        public void OnSelectedNodeChanged()
        {
            if (SelectedNode != null)
            {
                SelectedNode.IsExpanded = true;
            }
            eventAggregator.PublishOnUIThread(new SelectedExplorerItemChanged(SelectedNode));
        }

        void SelectDefaultEndpoint(ServiceControlExplorerItem serviceControlNode)
        {
            if (isStartingUp && serviceControlNode == null)
            {
                return;
            }

            if (!commandLineParser.ParsedOptions.EndpointName.IsEmpty())
            {
                bool found = false;
                foreach (var endpoint in serviceControlNode.Children)
                {
                    if (endpoint.Name.Equals(commandLineParser.ParsedOptions.EndpointName, StringComparison.OrdinalIgnoreCase))
                    {
                        SelectedNode = endpoint;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    SelectedNode = serviceControlNode;
                }
            }
            else
            {
                SelectedNode = serviceControlNode;
            }

            view?.ExpandSelectedNode();
        }

        ServiceControlExplorerItem SelectConnectedServiceControlNode(string url)
        {
            return Items.OfType<ServiceControlExplorerItem>().SingleOrDefault(ei => ei.Url.TrimEnd('/') == url.TrimEnd('/'));
        }

        public async Task ConnectToService(string url)
        {
            if (!url.IsValidUrl())
            {
                return;
            }

            var connectedNode = SelectConnectedServiceControlNode(url);
            if (connectedNode != null)
            {
                ExpandServiceControlNode(connectedNode);
                SelectDefaultEndpoint(connectedNode);
                return;
            }

            var available = false;
            var address = url;

            using (workNotifier.NotifyOfWork($"Verifying ServiceControl availability at {address}"))
            {
                var serviceControl = clientRegistry.Create(url);
                (available, address) = await serviceControl.IsAlive();
            }

            if (!available)
            {
                return;
            }

            clientRegistry.EnsureServiceControlClient(address);

            using (workNotifier.NotifyOfWork($"Connecting to ServiceControl at {address}"))
            {
                var node = AddServiceNode(address);
                await RefreshEndpoints(node);
                ExpandServiceControlNode(node);
                SelectDefaultEndpoint(node);
            }
        }

        public async Task RefreshAllEndpoints()
        {
            foreach (var serviceControlExplorerItem in ServiceControls)
            {
                await RefreshEndpoints(serviceControlExplorerItem);
            }
        }

        public async Task RefreshSelectedEndpoint()
        {
            var serviceControl = GetSelectedServiceControl();
            if (serviceControl != null)
            {
                await RefreshEndpoints(serviceControl);
            }
        }

        public async Task RefreshEndpoints(ServiceControlExplorerItem serviceControlNode)
        {
            if (serviceControlNode == null)
            {
                return;
            }

            serviceControlNode.Children.Clear();

            var serviceControl = clientRegistry.GetServiceControl(serviceControlNode.Url);
            var endpoints = await serviceControl.GetEndpoints();
            if (endpoints == null)
            {
                return;
            }

            var endpointInstancesGroupedByName = endpoints.OrderBy(x => x.Name).GroupBy(x => x.Name);
            foreach (var scaledOutEndpoint in endpointInstancesGroupedByName)
            {
                var instances = scaledOutEndpoint.ToList();
                var hostNames = instances.Select(instance => instance.HostDisplayName).Distinct();
                var tooltip = string.Join(", ", hostNames);
                serviceControlNode.Children.Add(new AuditEndpointExplorerItem(serviceControlNode, instances.First(), tooltip));
            }
        }

        public async Task DisconnectSelectedServiceControl()
        {
            var serviceControl = GetSelectedServiceControl();
            await DisconnectServiceControl(serviceControl);
        }

        public async Task DisconnectServiceControl(ServiceControlExplorerItem serviceControlNode)
        {
            if (serviceControlNode != null)
            {
                serviceControlNode.Children.Clear();
                if (Items.Contains(serviceControlNode))
                {
                    Items.Remove(serviceControlNode);
                }

                clientRegistry.RemoveServiceControlClient(serviceControlNode.Url);

                await eventAggregator.PublishOnUIThreadAsync(new ServiceControlDisconnected
                {
                    ExplorerItem = serviceControlNode
                });
            }
        }

        void ExpandServiceControlNode(ServiceControlExplorerItem serviceControlNode)
        {
            SelectedNode = serviceControlNode;
        }

        public void Handle(RequestSelectingEndpoint message)
        {
            var serviceControlNode = GetSelectedServiceControl();
            if (serviceControlNode?.EndpointExists(message.Endpoint) == true)
            {
                var node = serviceControlNode.GetEndpointNode(message.Endpoint);
                SelectedNode = node;
            }
        }

        public async Task Handle(ConfigurationUpdated message)
        {
            messageSelectionContext.SearchInProgress = true;
            await ConnectToService(commandLineParser.ParsedOptions.EndpointUri.ToString());
            await Parent.PerformSearchAndInitializeRefreshTimer();
        }
    }
}