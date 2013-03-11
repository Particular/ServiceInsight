using System.Linq;
using System.Threading.Tasks;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Caliburn.PresentationFramework.Views;
using NServiceBus.Profiler.Common;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Core.Management;
using NServiceBus.Profiler.Desktop.ScreenManager;

namespace NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer
{
    [View(typeof(IExplorerView))]
    public class EndpointExplorerViewModel : Screen, IEndpointExplorerViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManagerEx _windowManager;
        private readonly IManagementService _managementService;
        private bool _isFirstActivation = true;
        private IExplorerView _view;

        public EndpointExplorerViewModel(
            IEventAggregator eventAggregator, 
            IWindowManagerEx windowManager,
            IManagementService managementService)
        {
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;
            _managementService = managementService;
            Items = new BindableCollection<ExplorerItem>();
        }

        public virtual IObservableCollection<ExplorerItem> Items { get; private set; }

        public virtual ExplorerItem ServiceRoot
        {
            get { return Items.FirstOrDefault(x => x is EndpointExplorerItem); }
        }

        public virtual ExplorerItem AuditRoot
        {
            get { return ServiceRoot != null ? ServiceRoot.Children.OfType<AuditQueueExplorerItem>().First() : null; }
        }

        public virtual ExplorerItem ErrorRoot
        {
            get { return ServiceRoot != null ? ServiceRoot.Children.OfType<ErrorQueueExplorerItem>().First() : null; }
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

        public async override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            _view = view as IExplorerView;

            const string defaultAddress = "http://127.0.0.1:8888";
            if (!IsConnected)
            {
                var available = await ServiceAvailable(defaultAddress);
                if (available)
                {
                    ConnectToService(defaultAddress); //TODO: Connect to default API address
                }
            }
        }

        private async Task<bool> ServiceAvailable(string defaultAddress)
        {
            return await _managementService.IsAlive(defaultAddress);
        }

        private async void AddServiceNode()
        {
            if (ServiceRoot == null)
            {
                Items.Add(new EndpointExplorerItem(ServiceUrl));
            }

            ServiceRoot.Children.Clear();

            var endpoints = await _managementService.GetEndpoints(ServiceUrl);
            
            if(endpoints == null)
                return;

            foreach (var endpoint in endpoints)
            {
                ServiceRoot.Children.Add(new AuditQueueExplorerItem(endpoint));
            }

            _view.ExpandNode(ServiceRoot);
        }

        public virtual void OnSelectedNodeChanged()
        {
            var auditNode = SelectedNode as AuditQueueExplorerItem;
            if (auditNode != null)
            {
                _eventAggregator.Publish(new AuditQueueSelectedEvent
                {
                    Endpoint = auditNode.Endpoint
                });                
            }

            var errorNode = SelectedNode as ErrorQueueExplorerItem;
            if (errorNode != null)
            {
                _eventAggregator.Publish(new ErrorQueueSelectedEvent
                {
                    Endpoint = errorNode.Endpoint
                });
            }
        }

        public virtual void ConnectToService(string url)
        {
            Guard.NotNull(() => url, url);

            ServiceUrl = url;
            AddServiceNode();
        }

        public void FullRefresh()
        {
            
        }

        public void PartialRefresh()
        {
        }

        public void Handle(AutoRefreshBeatEvent message)
        {
            PartialRefresh();
        }

    }
}