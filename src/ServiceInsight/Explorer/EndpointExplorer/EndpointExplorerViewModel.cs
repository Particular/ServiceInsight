namespace ServiceInsight.Explorer.EndpointExplorer
{
    using System;
    using System.Linq;
    using Caliburn.Micro;
    using ServiceInsight.ExtensionMethods;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.Settings;
    using ServiceInsight.ServiceControl;
    using ServiceInsight.Settings;
    using ServiceInsight.Shell;
    using ServiceInsight.Startup;

    public class EndpointExplorerViewModel : Screen,
        IHandle<RequestSelectingEndpoint>
    {
        IEventAggregator eventAggregator;
        IWorkNotifier workNotifier;
        ISettingsProvider settingsProvider;
        IServiceControl serviceControl;
        NetworkOperations networkOperations;
        ServiceControlConnectionProvider connectionProvider;
        CommandLineArgParser commandLineParser;

        public EndpointExplorerViewModel(
            IEventAggregator eventAggregator,
            IWorkNotifier workNotifier,
            ISettingsProvider settingsProvider,
            ServiceControlConnectionProvider connectionProvider,
            CommandLineArgParser commandLineParser,
            IServiceControl serviceControl,
            NetworkOperations networkOperations)
        {
            this.eventAggregator = eventAggregator;
            this.workNotifier = workNotifier;
            this.settingsProvider = settingsProvider;
            this.serviceControl = serviceControl;
            this.networkOperations = networkOperations;
            this.connectionProvider = connectionProvider;
            this.commandLineParser = commandLineParser;
            Items = new BindableCollection<ExplorerItem>();
        }

        public IObservableCollection<ExplorerItem> Items { get; }

        public ServiceControlExplorerItem ServiceControlRoot => Items.OfType<ServiceControlExplorerItem>().FirstOrDefault();

        public ExplorerItem SelectedNode { get; set; }

        public string ServiceUrl { get; private set; }

        public new ShellViewModel Parent => (ShellViewModel)base.Parent;

        bool IsConnected => ServiceUrl != null;

        protected override void OnActivate()
        {
            base.OnActivate();

            if (IsConnected)
            {
                return;
            }

            var configuredConnection = GetConfiguredAddress();
            var existingConnection = connectionProvider.Url;
            var available = ServiceAvailable(configuredConnection);
            var connectTo = available ? configuredConnection : existingConnection;

            using (workNotifier.NotifyOfWork($"Trying to connect to ServiceControl at {connectTo}"))
            {
                ConnectToService(connectTo);

                SelectDefaultEndpoint();
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

        bool ServiceAvailable(string serviceUrl)
        {
            connectionProvider.ConnectTo(serviceUrl);

            var connected = serviceControl.IsAlive();

            return connected;
        }

        void AddServiceNode()
        {
            Items.Clear();
            Items.Add(new ServiceControlExplorerItem(ServiceUrl));
        }

        public void OnSelectedNodeChanged()
        {
            eventAggregator.PublishOnUIThread(new SelectedExplorerItemChanged(SelectedNode));
        }

        void SelectDefaultEndpoint()
        {
            if (ServiceControlRoot == null)
            {
                return;
            }

            if (!commandLineParser.ParsedOptions.EndpointName.IsEmpty())
            {
                foreach (var endpoint in ServiceControlRoot.Children)
                {
                    if (endpoint.Name.Equals(commandLineParser.ParsedOptions.EndpointName, StringComparison.OrdinalIgnoreCase))
                    {
                        //SelectedNode = endpoint;
                        SelectedNode = ServiceControlRoot;
                        break;
                    }
                }
            }
            else
            {
                SelectedNode = ServiceControlRoot;
            }
        }

        public void ConnectToService(string url)
        {
            if (url == null)
            {
                return;
            }

            connectionProvider.ConnectTo(url);
            ServiceUrl = url;
            AddServiceNode();
            RefreshData();
            ExpandServiceNode();
        }

        public void RefreshData()
        {
            if (ServiceControlRoot == null)
            {
                TryReconnectToServiceControl();
            }
            if (ServiceControlRoot == null)
            {
                return;
            }

            var endpoints = serviceControl.GetEndpoints();
            if (endpoints == null)
            {
                return;
            }

            ServiceControlRoot.Children.Clear();

            var endpointInstancesGroupedByName = endpoints.OrderBy(x => x.Name).GroupBy(x => x.Name);
            foreach (var scaledOutEndpoint in endpointInstancesGroupedByName)
            {
                var instances = scaledOutEndpoint.ToList();
                var hostNames = instances.Select(instance => instance.HostDisplayName).Distinct();
                var tooltip = string.Join(", ", hostNames);
                ServiceControlRoot.Children.Add(new AuditEndpointExplorerItem(instances.First(), tooltip));
            }
        }

        void TryReconnectToServiceControl()
        {
            ConnectToService(GetConfiguredAddress());
        }

        public void Navigate(string navigateUri)
        {
            networkOperations.Browse(navigateUri);
        }

        void ExpandServiceNode()
        {
            ServiceControlRoot.IsExpanded = true;
            SelectedNode = ServiceControlRoot;
        }

        public void Handle(RequestSelectingEndpoint message)
        {
            if (ServiceControlRoot.EndpointExists(message.Endpoint))
            {
                var node = ServiceControlRoot.GetEndpointNode(message.Endpoint);
                SelectedNode = node;
            }
        }
    }
}