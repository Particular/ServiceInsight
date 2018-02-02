namespace ServiceInsight.Explorer.EndpointExplorer
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
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

        protected override async void OnActivate()
        {
            base.OnActivate();

            if (IsConnected)
            {
                return;
            }

            var configuredConnection = GetConfiguredAddress();
            var existingConnection = connectionProvider.Url;
            var available = await ServiceAvailable(configuredConnection);
            var connectTo = available ? configuredConnection : existingConnection;

            using (workNotifier.NotifyOfWork($"Trying to connect to ServiceControl at {connectTo}"))
            {
                await ConnectToService(connectTo);

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

        async Task<bool> ServiceAvailable(string serviceUrl)
        {
            if (!serviceUrl.IsValidUrl())
            {
                return false;
            }

            connectionProvider.ConnectTo(serviceUrl);

            var connected = await serviceControl.IsAlive();

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

        public async Task ConnectToService(string url)
        {
            if (!url.IsValidUrl())
            {
                return;
            }

            connectionProvider.ConnectTo(url);
            ServiceUrl = url;
            AddServiceNode();
            await RefreshData();
            ExpandServiceNode();
        }

        public async Task RefreshData()
        {
            if (ServiceControlRoot == null)
            {
                await TryReconnectToServiceControl();
            }
            if (ServiceControlRoot == null)
            {
                return;
            }

            var endpoints = await serviceControl.GetEndpoints();
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

        Task TryReconnectToServiceControl()
        {
            return ConnectToService(GetConfiguredAddress());
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
            if (ServiceControlRoot?.EndpointExists(message.Endpoint) == true)
            {
                var node = ServiceControlRoot.GetEndpointNode(message.Endpoint);
                SelectedNode = node;
            }
        }
    }
}