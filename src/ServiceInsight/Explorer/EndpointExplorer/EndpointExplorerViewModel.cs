using System.Collections.Generic;

namespace ServiceInsight.Explorer.EndpointExplorer
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using ExtensionMethods;
    using Framework;
    using Framework.Events;
    using Framework.Settings;
    using ServiceControl;
    using Settings;
    using Shell;
    using Startup;

    public class EndpointExplorerViewModel : Screen,
        IHandle<RequestSelectingEndpoint>,
        IHandleWithTask<ConfigurationUpdated>
    {
        readonly IEventAggregator eventAggregator;
        readonly IWorkNotifier workNotifier;
        readonly ISettingsProvider settingsProvider;
        readonly CommandLineArgParser commandLineParser;
        readonly ServiceControlClientRegistry clientRegistry;
        bool isStartingUp;
        
        public EndpointExplorerViewModel(
            IEventAggregator eventAggregator,
            IWorkNotifier workNotifier,
            ISettingsProvider settingsProvider,
            CommandLineArgParser commandLineParser,
            ServiceControlClientRegistry clientRegistry)
        {
            this.eventAggregator = eventAggregator;
            this.workNotifier = workNotifier;
            this.settingsProvider = settingsProvider;
            this.clientRegistry = clientRegistry;
            this.commandLineParser = commandLineParser;
            Items = new BindableCollection<ExplorerItem>();
        }

        public IObservableCollection<ExplorerItem> Items { get; }

        public ExplorerItem SelectedNode { get; set; }

        public new ShellViewModel Parent => (ShellViewModel)base.Parent;

        bool IsConnected => HasConnectedToAnyServiceControl;

        public bool HasSelectedServiceControl => GetSelectedServiceControl() != null;

        public bool HasConnectedToAnyServiceControl => ServiceControls.Any();

        public ServiceControlExplorerItem GetSelectedServiceControl()
        {
            var selected = SelectedNode;
            if (selected != null)
            {
                if (selected is ServiceControlExplorerItem)
                {
                    return (ServiceControlExplorerItem)selected;
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
        }

        public async Task ConnectToService(string url)
        {
            if (!url.IsValidUrl())
            {
                return;
            }

            var available = false;
            using (workNotifier.NotifyOfWork($"Verifying ServiceControl availability at {url}"))
            {
                clientRegistry.EnsureServiceControlClient(url);
                var serviceControl = clientRegistry.GetServiceControl(url);
                available = await serviceControl.IsAlive();
            }

            if (!available) 
                return;

            using (workNotifier.NotifyOfWork($"Connecting to ServiceControl at {url}"))
            {
                var node = AddServiceNode(url);
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
            // if (!HasConnectedToAnyServiceControl)
            // {
            //     await ConnectToKnownInstance();
            // }
            //
            // if (!HasConnectedToAnyServiceControl)
            // {
            //     return;
            // }
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
        
        public Task DisconnectServiceControl(ServiceControlExplorerItem serviceControlNode)
        {
            if (serviceControlNode != null)
            {
                serviceControlNode.Children.Clear();
                if (Items.Contains(serviceControlNode))
                {
                    Items.Remove(serviceControlNode);
                }
            }
            
            return Task.CompletedTask;
        }

        // Task ConnectToKnownInstance()
        // {
        //     return ConnectToService(GetConfiguredAddress());
        // }

        // public void Navigate(string navigateUri)
        // {
        //     networkOperations.Browse(navigateUri);
        // }
        void ExpandServiceControlNode(ServiceControlExplorerItem serviceControlNode)
        {
            serviceControlNode.IsExpanded = true;
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
            await ConnectToService(commandLineParser.ParsedOptions.EndpointUri.ToString());
            await Parent.PostConfigurationUpdate();
        }
    }
}