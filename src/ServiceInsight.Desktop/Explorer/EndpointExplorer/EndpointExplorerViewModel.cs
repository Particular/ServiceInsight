namespace Particular.ServiceInsight.Desktop.Explorer.EndpointExplorer
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Core;
    using Core.Settings;
    using Events;
    using ExtensionMethods;
    using ServiceControl;
    using Settings;
    using Shell;
    using Startup;

    public class EndpointExplorerViewModel : Screen,
        IExplorerViewModel,
        IHandle<RequestSelectingEndpoint>
    {
        IEventAggregator eventAggregator;
        ISettingsProvider settingsProvider;
        DefaultServiceControl serviceControl;
        NetworkOperations networkOperations;
        ServiceControlConnectionProvider connectionProvider;
        CommandLineArgParser commandLineParser;
        bool isFirstActivation = true;
        IExplorerView view;

        public EndpointExplorerViewModel(
            IEventAggregator eventAggregator,
            ISettingsProvider settingsProvider,
            ServiceControlConnectionProvider connectionProvider,
            CommandLineArgParser commandLineParser,
            DefaultServiceControl serviceControl,
            NetworkOperations networkOperations)
        {
            this.eventAggregator = eventAggregator;
            this.settingsProvider = settingsProvider;
            this.serviceControl = serviceControl;
            this.networkOperations = networkOperations;
            this.connectionProvider = connectionProvider;
            this.commandLineParser = commandLineParser;
            Items = new BindableCollection<ExplorerItem>();
        }

        public IObservableCollection<ExplorerItem> Items { get; private set; }

        public ServiceControlExplorerItem ServiceControlRoot
        {
            get
            {
                return Items.OfType<ServiceControlExplorerItem>().FirstOrDefault();
            }
        }

        public AuditEndpointExplorerItem AuditRoot
        {
            get { return ServiceControlRoot != null ? ServiceControlRoot.Children.OfType<AuditEndpointExplorerItem>().First() : null; }
        }

        public ErrorEndpointExplorerItem ErrorRoot
        {
            get { return ServiceControlRoot != null ? ServiceControlRoot.Children.OfType<ErrorEndpointExplorerItem>().First() : null; }
        }

        public ExplorerItem SelectedNode { get; set; }

        public string ServiceUrl { get; private set; }

        public new ShellViewModel Parent
        {
            get { return (ShellViewModel)base.Parent; }
        }

        bool IsConnected
        {
            get { return ServiceUrl != null; }
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            if (isFirstActivation)
            {
                this.view.ExpandNode(ServiceControlRoot);
                isFirstActivation = false;
            }
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            this.view = view as IExplorerView;
        }

        protected async override void OnActivate()
        {
            base.OnActivate();

            if (IsConnected) return;

            var configuredConnection = GetConfiguredAddress();
            var existingConnection = connectionProvider.Url;
            var available = await ServiceAvailable(configuredConnection);
            var connectTo = available ? configuredConnection : existingConnection;

            eventAggregator.Publish(new WorkStarted("Trying to connect to ServiceControl at {0}", connectTo));

            await ConnectToService(connectTo);

            SelectDefaultEndpoint();

            eventAggregator.Publish(new WorkFinished());
        }

        string GetConfiguredAddress()
        {
            if (commandLineParser.ParsedOptions.EndpointUri == null)
            {
                var appSettings = settingsProvider.GetSettings<ProfilerSettings>();
                if (appSettings != null && appSettings.LastUsedServiceControl != null)
                    return appSettings.LastUsedServiceControl;

                var managementConfig = settingsProvider.GetSettings<ServiceControlSettings>();
                return string.Format("http://localhost:{0}/api", managementConfig.Port);
            }

            return commandLineParser.ParsedOptions.EndpointUri.ToString();
        }

        async Task<bool> ServiceAvailable(string serviceUrl)
        {
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
            eventAggregator.Publish(new SelectedExplorerItemChanged(SelectedNode));
        }

        void SelectDefaultEndpoint()
        {
            if (ServiceControlRoot == null) return;

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
            if (url == null)
                return;

            connectionProvider.ConnectTo(url);
            ServiceUrl = url;
            AddServiceNode();
            await RefreshData();
            ExpandServiceNode();
        }

        public async Task RefreshData()
        {
            if (ServiceControlRoot == null) await TryReconnectToServiceControl();
            if (ServiceControlRoot == null) return; //TODO: DO we need to check twice? Root node should have been added at this stage.

            var endpoints = await serviceControl.GetEndpoints();
            if (endpoints == null) return;

            foreach (var endpoint in endpoints.OrderBy(e => e.Name))
            {
                if (!ServiceControlRoot.EndpointExists(endpoint))
                {
                    ServiceControlRoot.Children.Add(new AuditEndpointExplorerItem(endpoint));
                }
            }

            //TODO: Remove non-existing endpoints efficiently
        }

        async Task TryReconnectToServiceControl()
        {
            await ConnectToService(GetConfiguredAddress());
        }

        public void Navigate(string navigateUri)
        {
            networkOperations.Browse(navigateUri);
        }

        void ExpandServiceNode()
        {
            view.ExpandNode(ServiceControlRoot);
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