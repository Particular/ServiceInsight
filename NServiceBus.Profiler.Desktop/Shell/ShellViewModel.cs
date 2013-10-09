using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Threading;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Filters;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Core.Licensing;
using NServiceBus.Profiler.Desktop.Core.Settings;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Explorer;
using NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer;
using NServiceBus.Profiler.Desktop.Explorer.QueueExplorer;
using NServiceBus.Profiler.Desktop.ExtensionMethods;
using NServiceBus.Profiler.Desktop.LogWindow;
using NServiceBus.Profiler.Desktop.MessageFlow;
using NServiceBus.Profiler.Desktop.MessageList;
using NServiceBus.Profiler.Desktop.MessageProperties;
using NServiceBus.Profiler.Desktop.MessageViewers;
using NServiceBus.Profiler.Desktop.Options;
using NServiceBus.Profiler.Desktop.ScreenManager;
using NServiceBus.Profiler.Desktop.Settings;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public class ShellViewModel : Conductor<IScreen>.Collection.AllActive, IShellViewModel
    {
        private readonly IAppCommands _appCommander;
        private readonly IScreenFactory _screenFactory;
        private readonly IWindowManagerEx _windowManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILicenseManager _licenseManager;
        private readonly ISettingsProvider _settingsProvider;
        private int _workCounter;
        private DispatcherTimer _refreshTimer;
        private DispatcherTimer _idleTimer;
        
        public const string UnlicensedStatusMessage = "Unlicensed version: {0} left";
        public const string LicensedStatusMessage = "Registered to '{0}'";

        public ShellViewModel(
            IAppCommands appCommander,
            IScreenFactory screenFactory,
            IWindowManagerEx windowManager,
            IQueueExplorerViewModel queueExplorer, 
            IEndpointExplorerViewModel endpointExplorer,
            IMessageListViewModel messages,
            IStatusBarManager statusBarManager,
            IEventAggregator eventAggregator,
            ILicenseManager licenseManager,
            IMessageFlowViewModel messageFlow,
            IMessageBodyViewModel messageBodyViewer,
            ISettingsProvider settingsProvider,
            IMessagePropertiesViewModel messageProperties,
            ILogWindowViewModel logWindow)
        {
            _appCommander = appCommander;
            _screenFactory = screenFactory;
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;
            _licenseManager = licenseManager;
            _settingsProvider = settingsProvider;
            MessageProperties = messageProperties;
            MessageFlow = messageFlow;
            StatusBarManager = statusBarManager;
            QueueExplorer = queueExplorer;
            EndpointExplorer = endpointExplorer;
            MessageBody = messageBodyViewer;
            Messages = messages;
            LogWindow = logWindow;

            Items.Add(endpointExplorer);
            Items.Add(messageBodyViewer);
            Items.Add(messageFlow);
            Items.Add(queueExplorer);
            Items.Add(messages);

            InitializeAutoRefreshTimer();
            InitializeIdleTimer();
        }

        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            View = (IShellView)view;

            DisplayName = GetProductName();
            StatusBarManager.Done();
            RestoreLayout();
        }

        public virtual void Deactivate(bool close )
        {
            base.OnDeactivate(close);
            _refreshTimer.Stop();
            SaveLayout();
        }

        private void SaveLayout()
        {
            View.OnSaveLayout(_settingsProvider);
        }

        private void RestoreLayout()
        {
            View.OnRestoreLayout(_settingsProvider);
        }

        public virtual void ResetLayout()
        {
            View.OnResetLayout(_settingsProvider);
        }

        public virtual bool AutoRefresh { get; set; }

        public virtual IShellView View { get; private set; }

        public virtual IMessagePropertiesViewModel MessageProperties { get; private set; }

        public virtual IQueueExplorerViewModel QueueExplorer { get; private set; }

        public virtual IEndpointExplorerViewModel EndpointExplorer { get; private set; }

        public virtual IMessageListViewModel Messages { get; private set; }

        public virtual IMessageFlowViewModel MessageFlow { get; private set; }

        public virtual IMessageBodyViewModel MessageBody { get; private set; }

        public virtual IStatusBarManager StatusBarManager { get; private set; }

        public virtual ILogWindowViewModel LogWindow { get; private set; }

        public virtual ExplorerItem SelectedExplorerItem { get; private set; }

        public virtual bool WorkInProgress
        {
            get { return _workCounter > 0; }
        }

        public virtual void ShutDown()
        {
            _appCommander.ShutdownImmediately();
        }

        public virtual void About()
        {
            _windowManager.ShowDialog<AboutViewModel>();
        }

        public virtual void Help()
        {
            throw new NotImplementedException("This feature is not yet implemented.");
        }

        public void Options()
        {
            _windowManager.ShowDialog<OptionsViewModel>();
        }

        [AutoCheckAvailability]
        public virtual void ConnectToMachine()
        {
            var machineViewModel = _screenFactory.CreateScreen<ConnectToMachineViewModel>();
            var result = _windowManager.ShowDialog(machineViewModel);

            if(result.GetValueOrDefault(false))
            {
                QueueExplorer.ConnectToQueue(machineViewModel.ComputerName);
            }
        }

        [AutoCheckAvailability]
        public virtual async void ConnectToManagementService()
        {
            var connectionViewModel = _screenFactory.CreateScreen<ManagementConnectionViewModel>();
            var result = _windowManager.ShowDialog(connectionViewModel);

            if (result.GetValueOrDefault(false))
            {
                await EndpointExplorer.ConnectToService(connectionViewModel.ServiceUrl);
                _eventAggregator.Publish(new WorkFinished("Connected to Management API Version {0}", connectionViewModel.Version));
            }
        }

        [AutoCheckAvailability]
        public virtual async void DeleteSelectedMessages()
        {
            await Messages.DeleteSelectedMessages();
        }

        [AutoCheckAvailability]
        public virtual async void PurgeCurrentQueue()
        {
            await Messages.PurgeQueue();
        }

        [AutoCheckAvailability]
        public virtual void DeleteCurrentQueue()
        {
            QueueExplorer.DeleteSelectedQueue();
        }

        [AutoCheckAvailability]
        public virtual async void RefreshAll()
        {
            await Messages.RefreshMessages();
            await RefreshExplorer();
        }

        private async Task RefreshExplorer()
        {
            if (SelectedExplorerItem.IsEndpointExplorerSelected())
            {
                await EndpointExplorer.PartialRefresh();
            }

            if (SelectedExplorerItem.IsQueueExplorerSelected())
            {
                await QueueExplorer.PartialRefresh();
            }
        }

        [AutoCheckAvailability]
        public virtual void ImportMessage()
        {
            throw new NotImplementedException("This feature is not yet implemented.");
        }

        [AutoCheckAvailability]
        public virtual void ExportMessage()
        {
            throw new NotImplementedException("This feature is not yet implemented.");
        }

        [AutoCheckAvailability]
        public virtual void CreateQueue()
        {
            var screen = _screenFactory.CreateScreen<IQueueCreationViewModel>();
            var result = _windowManager.ShowDialog(screen);

            if(result.GetValueOrDefault(false))
            {
                QueueExplorer.FullRefresh();
            }
        }

        [AutoCheckAvailability]
        public virtual void CreateMessage()
        {
            throw new NotImplementedException("This feature is not yet implemented.");
        }

        public virtual void Register()
        {
            _windowManager.ShowDialog<ILicenseRegistrationViewModel>();
            DisplayRegistrationStatus();
        }

        public void OnAutoRefreshQueuesChanged()
        {
            
        }

        public virtual bool CanCreateMessage
        {
            get { return QueueExplorer.SelectedQueue != null && !WorkInProgress; }
        }

        public virtual bool CanRefreshQueues
        {
            get { return !WorkInProgress; }
        }

        public virtual bool CanPurgeCurrentQueue
        {
            get
            {
                return Messages.SelectedQueue != null &&
                       !WorkInProgress &&
                       SelectedExplorerItem.IsQueueExplorerSelected();
            }
        }

        public virtual bool CanDeleteCurrentQueue
        {
            get
            {
                return Messages.SelectedQueue != null &&
                       !WorkInProgress &&
                       SelectedExplorerItem.IsQueueExplorerSelected();
            }
        }

        public virtual bool CanDeleteSelectedMessages
        {
            get
            {
                return !WorkInProgress &&
                       Messages.FocusedMessage != null &&
                       SelectedExplorerItem.IsQueueExplorerSelected();
            }
        }

        public virtual bool CanCreateQueue
        {
            get
            {
                return !QueueExplorer.ConnectedToAddress.IsEmpty() &&
                       !WorkInProgress &&
                       SelectedExplorerItem.IsQueueExplorerSelected();
            }
        }

        public virtual bool CanConnectToMachine
        {
            get { return !WorkInProgress; }
        }

        public virtual bool CanConnectToManagementService
        {
            get { return !WorkInProgress; }
        }

        public virtual bool CanExportMessage
        {
            get { return !WorkInProgress && Messages.SelectedMessages.Count > 0 && false; } //TODO: Implement message export
        }

        public virtual bool CanImportMessage
        {
            get { return !WorkInProgress && false; } //TODO: Implement message import
        }
        
        private void InitializeIdleTimer()
        {
            _idleTimer = new DispatcherTimer(DispatcherPriority.Loaded) {Interval = TimeSpan.FromSeconds(10)};
            _idleTimer.Tick += (s, e) => OnApplicationIdle();
            _idleTimer.Start();
        }

        private void InitializeAutoRefreshTimer()
        {
            var appSetting = _settingsProvider.GetSettings<ProfilerSettings>();
            _refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(appSetting.AutoRefreshTimer) };
            _refreshTimer.Tick += (s, e) => OnAutoRefreshing();
            _refreshTimer.Start();
        }

        internal void OnApplicationIdle()
        {
            if (_idleTimer != null)
                _idleTimer.Stop();

            ValidateLicense();
        }

        internal void OnAutoRefreshing()
        {
            if (!AutoRefresh || WorkInProgress)
                return;

            _eventAggregator.Publish(new AutoRefreshBeat());
        }
        
        private void ValidateLicense()
        {
            if (_licenseManager.TrialExpired)
            {
                RegisterLicense();
            }

            DisplayRegistrationStatus();
        }

        private void DisplayRegistrationStatus()
        {
            var license = _licenseManager.CurrentLicense;
            if (license.LicenseType == ProfilerLicenseTypes.Standard)
            {
                StatusBarManager.SetRegistrationInfo(LicensedStatusMessage, license.RegisteredTo);
            }
            else
            {
                StatusBarManager.SetRegistrationInfo(UnlicensedStatusMessage, ("day").PluralizeWord(_licenseManager.GetRemainingTrialDays()));
            }
        }

        private void RegisterLicense()
        {
            var model = _screenFactory.CreateScreen<ILicenseRegistrationViewModel>();
            var result = _windowManager.ShowDialog(model);

            if (!result.GetValueOrDefault(false))
            {
                ShutDown();
            }
        }

        private void NotifyPropertiesChanged()
        {
            NotifyOfPropertyChange(() => WorkInProgress);
            NotifyOfPropertyChange(() => CanConnectToMachine);
            NotifyOfPropertyChange(() => CanConnectToManagementService);
            NotifyOfPropertyChange(() => CanCreateMessage);
            NotifyOfPropertyChange(() => CanCreateQueue);
            NotifyOfPropertyChange(() => CanDeleteCurrentQueue);
            NotifyOfPropertyChange(() => CanDeleteSelectedMessages);
            NotifyOfPropertyChange(() => CanExportMessage);
            NotifyOfPropertyChange(() => CanImportMessage);
            NotifyOfPropertyChange(() => CanPurgeCurrentQueue);
            NotifyOfPropertyChange(() => CanRefreshQueues);
        }

        private string GetProductName()
        {
            var productAttribute = GetType().Assembly.GetAttribute<AssemblyProductAttribute>();
            return productAttribute.Product;
        }

        public virtual void Handle(WorkStarted @event)
        {
            _workCounter++;
            NotifyPropertiesChanged();
        }

        public virtual void Handle(WorkFinished @event)
        {
            if (_workCounter <= 0) 
                return;

            _workCounter--;
            NotifyPropertiesChanged();
        }

        public virtual void Handle(SelectedExplorerItemChanged @event)
        {
            SelectedExplorerItem = @event.SelectedExplorerItem;
        }

        public virtual void Handle(AsyncOperationFailedEvent message)
        {
            StatusBarManager.SetFailStatusMessage("Operation Failed: {0}", message.Message);
        }
    }
}