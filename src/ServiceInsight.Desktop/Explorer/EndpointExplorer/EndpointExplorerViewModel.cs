using System;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Caliburn.PresentationFramework.Views;
using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.Core.Settings;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.ExtensionMethods;
using NServiceBus.Profiler.Desktop.ServiceControl;
using NServiceBus.Profiler.Desktop.Settings;
using NServiceBus.Profiler.Desktop.Shell;
using NServiceBus.Profiler.Desktop.Startup;

namespace NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer
{
    [View(typeof(EndpointExplorerView))]
    public class EndpointExplorerViewModel : Screen, IEndpointExplorerViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IServiceControl _serviceControl;
        private readonly INetworkOperations _networkOperations;
        private readonly IServiceControlConnectionProvider _connectionProvider;
        private readonly ICommandLineArgParser _commandLineParser;
        private bool _isFirstActivation = true;
        private IExplorerView _view;

        public EndpointExplorerViewModel(
            IEventAggregator eventAggregator, 
            ISettingsProvider settingsProvider,
            IServiceControlConnectionProvider connectionProvider,
            ICommandLineArgParser commandLineParser,
            IServiceControl serviceControl,
            INetworkOperations networkOperations)
        {
            _eventAggregator = eventAggregator;
            _settingsProvider = settingsProvider;
            _serviceControl = serviceControl;
            _networkOperations = networkOperations;
            _connectionProvider = connectionProvider;
            _commandLineParser = commandLineParser;
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

        public new IShellViewModel Parent
        {
            get { return (IShellViewModel)base.Parent; }
        }

        private bool IsConnected
        {
            get { return ServiceUrl != null; }
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            if (_isFirstActivation)
            {
                _view.ExpandNode(ServiceControlRoot);
                _isFirstActivation = false;
            }
        }

        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            _view = view as IExplorerView;
        }

        protected async override void OnActivate()
        {
            base.OnActivate();

            if (IsConnected) return;

            var configuredConnection = GetConfiguredAddress();
            var existingConnection = _connectionProvider.Url;
            var available = await ServiceAvailable(configuredConnection);
            var connectTo = available ? configuredConnection : existingConnection;

            _eventAggregator.Publish(new WorkStarted("Trying to connect to ServiceControl at {0}", connectTo));

            await ConnectToService(connectTo);
            
            SelectDefaultEndpoint();

            _eventAggregator.Publish(new WorkFinished());
        }

        private string GetConfiguredAddress()
        {
            if (_commandLineParser.ParsedOptions.EndpointUri == null)
            {
                var appSettings = _settingsProvider.GetSettings<ProfilerSettings>();
                if (appSettings != null && appSettings.LastUsedServiceControl != null)
                    return appSettings.LastUsedServiceControl;

                var managementConfig = _settingsProvider.GetSettings<ServiceControlSettings>();
                return string.Format("http://localhost:{0}/api", managementConfig.Port);
            }

            return _commandLineParser.ParsedOptions.EndpointUri.ToString();
        }

        private async Task<bool> ServiceAvailable(string serviceUrl)
        {
            _connectionProvider.ConnectTo(serviceUrl);

            var connected = await _serviceControl.IsAlive();

            return connected;
        }

        private void AddServiceNode()
        {
            Items.Clear();
            Items.Add(new ServiceControlExplorerItem(ServiceUrl));
        }

        public void OnSelectedNodeChanged()
        {
            _eventAggregator.Publish(new SelectedExplorerItemChanged(SelectedNode));
        }

        private void SelectDefaultEndpoint()
        {
            if (ServiceControlRoot == null) return;

            if (!_commandLineParser.ParsedOptions.EndpointName.IsEmpty())
            {
                foreach (var endpoint in ServiceControlRoot.Children)
                {
                    if (endpoint.Name.Equals(_commandLineParser.ParsedOptions.EndpointName, StringComparison.OrdinalIgnoreCase))
                    {
                        //SelectedNode = endpoint;
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
            if(url == null)
                return;

            _connectionProvider.ConnectTo(url);
            ServiceUrl = url;
            AddServiceNode();
            await RefreshData();
            ExpandServiceNode();
        }

        public async Task RefreshData()
        {
            if (ServiceControlRoot == null) await TryReconnectToServiceControl();
            if (ServiceControlRoot == null) return; //TODO: DO we need to check twice? Root node should have been added at this stage.

            var endpoints = await _serviceControl.GetEndpoints();
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

        private async Task TryReconnectToServiceControl()
        {
            await ConnectToService(GetConfiguredAddress());
        }

        public void Navigate(string navigateUri)
        {
            _networkOperations.Browse(navigateUri);
        }

        private void ExpandServiceNode()
        {
            _view.ExpandNode(ServiceControlRoot);
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