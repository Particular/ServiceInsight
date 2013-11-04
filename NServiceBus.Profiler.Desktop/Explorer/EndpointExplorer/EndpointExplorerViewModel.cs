using System.Linq;
using System.Threading.Tasks;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Caliburn.PresentationFramework.Views;
using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.Core.Settings;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Management;
using NServiceBus.Profiler.Desktop.Settings;

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
        private bool _isFirstActivation = true;
        private IExplorerView _view;

        public EndpointExplorerViewModel(
            IEventAggregator eventAggregator, 
            ISettingsProvider settingsProvider,
            IServiceControlConnectionProvider connectionProvider,
            IServiceControl serviceControl,
            INetworkOperations networkOperations)
        {
            _eventAggregator = eventAggregator;
            _settingsProvider = settingsProvider;
            _serviceControl = serviceControl;
            _networkOperations = networkOperations;
            _connectionProvider = connectionProvider;
            Items = new BindableCollection<ExplorerItem>();
        }

        public virtual IObservableCollection<ExplorerItem> Items { get; private set; }

        public virtual ServiceExplorerItem ServiceRoot
        {
            get { return Items.OfType<ServiceExplorerItem>().FirstOrDefault(); }
        }

        public virtual AuditEndpointExplorerItem AuditRoot
        {
            get { return ServiceRoot != null ? ServiceRoot.Children.OfType<AuditEndpointExplorerItem>().First() : null; }
        }

        public virtual ErrorEndpointExplorerItem ErrorRoot
        {
            get { return ServiceRoot != null ? ServiceRoot.Children.OfType<ErrorEndpointExplorerItem>().First() : null; }
        }

        public int SelectedRowHandle { get; set; }

        public virtual ExplorerItem SelectedNode { get; set; }

        public virtual string ServiceUrl { get; private set; }

        private bool IsConnected
        {
            get { return ServiceUrl != null; }
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            if (_isFirstActivation)
            {
                _view.ExpandNode(ServiceRoot);
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

            var configuredAddress = GetConfiguredAddress();
            var existingUrl = _connectionProvider.Url;
            var available = await ServiceAvailable(configuredAddress);
            var connectTo = available ? configuredAddress : existingUrl;

            _eventAggregator.Publish(new WorkStarted("Trying to connect to ServiceControl at {0}", connectTo));

            await ConnectToService(connectTo);

            _eventAggregator.Publish(new WorkFinished());
        }

        private string GetConfiguredAddress()
        {
            var appSettings = _settingsProvider.GetSettings<ProfilerSettings>();
            if (appSettings != null && appSettings.LastUsedManagementApi != null)
                return appSettings.LastUsedManagementApi;

            var managementConfig = _settingsProvider.GetSettings<ServiceControlSettings>();
            return string.Format("http://localhost:{0}/api", managementConfig.Port);
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
            Items.Add(new ServiceExplorerItem(ServiceUrl));
        }

        public virtual void OnSelectedNodeChanged()
        {
            _eventAggregator.Publish(new SelectedExplorerItemChanged(SelectedNode));
        }

        public async Task ConnectToService(string url)
        {
            if(url == null)
                return;

            _connectionProvider.ConnectTo(url);
            ServiceUrl = url;
            AddServiceNode();
            await RefreshEndpoints();
            ExpandServiceNode();
        }

        public async Task FullRefresh()
        {
            await Task.Run(() => { });
        }

        public async Task PartialRefresh()
        {
            await RefreshEndpoints();
        }

        public void Navigate(string navigateUri)
        {
            _networkOperations.Browse(navigateUri);
        }

        public async void Handle(AutoRefreshBeat message)
        {
            await PartialRefresh();
        }

        private async Task RefreshEndpoints()
        {
            var endpoints = await _serviceControl.GetEndpoints();

            if (endpoints == null)
                return;

            foreach (var endpoint in endpoints)
            {
                if (!ServiceRoot.EndpointExists(endpoint))
                {
                    ServiceRoot.Children.Add(new AuditEndpointExplorerItem(endpoint));
                }
            }
        }

        private void ExpandServiceNode()
        {
            _view.ExpandNode(ServiceRoot);
        }
    }
}