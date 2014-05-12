namespace Particular.ServiceInsight.Desktop.Shell
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Filters;
    using Caliburn.PresentationFramework.Screens;
    using Core.Licensing;
    using Core.Settings;
    using Core.UI.ScreenManager;
    using Events;
    using Explorer;
    using Explorer.EndpointExplorer;
    using Explorer.QueueExplorer;
    using ExtensionMethods;
    using LogWindow;
    using MessageFlow;
    using MessageHeaders;
    using MessageList;
    using MessageProperties;
    using MessageViewers;
    using Options;
    using Saga;
    using Settings;
    using Startup;

    public class ShellViewModel : Conductor<IScreen>.Collection.AllActive, IShellViewModel
    {
        private readonly IAppCommands appCommander;
        private readonly IScreenFactory screenFactory;
        private readonly IWindowManagerEx windowManager;
        private readonly IEventAggregator eventAggregator;
        private readonly AppLicenseManager licenseManager;
        private readonly ISettingsProvider settingsProvider;
        private readonly ICommandLineArgParser comandLineArgParser;
        private int workCounter;
        private DispatcherTimer refreshTimer;
        private DispatcherTimer idleTimer;
        
        public ShellViewModel(
            IAppCommands appCommander,
            IScreenFactory screenFactory,
            IWindowManagerEx windowManager,
            IQueueExplorerViewModel queueExplorer, 
            IEndpointExplorerViewModel endpointExplorer,
            IMessageListViewModel messages,
            IStatusBarManager statusBarManager,
            IEventAggregator eventAggregator,
            AppLicenseManager licenseManager,
            IMessageFlowViewModel messageFlow,
            ISagaWindowViewModel sagaWindow,
            IMessageBodyViewModel messageBodyViewer,
            IMessageHeadersViewModel messageHeadersViewer,
            ISettingsProvider settingsProvider,
            IMessagePropertiesViewModel messageProperties,
            ILogWindowViewModel logWindow,
            ICommandLineArgParser comandLineArgParser)
        {
            this.appCommander = appCommander;
            this.screenFactory = screenFactory;
            this.windowManager = windowManager;
            this.eventAggregator = eventAggregator;
            this.licenseManager = licenseManager;
            this.settingsProvider = settingsProvider;
            this.comandLineArgParser = comandLineArgParser;
            MessageProperties = messageProperties;
            MessageFlow = messageFlow;
            SagaWindow = sagaWindow;
            StatusBarManager = statusBarManager;
            QueueExplorer = queueExplorer;
            EndpointExplorer = endpointExplorer;
            MessageHeaders = messageHeadersViewer;
            MessageBody = messageBodyViewer;
            Messages = messages;
            LogWindow = logWindow;

            Items.Add(queueExplorer);
            Items.Add(endpointExplorer);
            Items.Add(messages);
            Items.Add(messageHeadersViewer);
            Items.Add(messageBodyViewer);
            Items.Add(messageFlow);

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

        public void Deactivate(bool close)
        {
            base.OnDeactivate(close);
            refreshTimer.Stop();
            SaveLayout();
        }

        private void SaveLayout()
        {
            if (!comandLineArgParser.ParsedOptions.ResetLayout)
            {
                View.OnSaveLayout(settingsProvider);
            }
        }

        private void RestoreLayout()
        {
            if (!comandLineArgParser.ParsedOptions.ResetLayout)
            {
                View.OnRestoreLayout(settingsProvider);
            }
        }

        public void ResetLayout()
        {
            View.OnResetLayout(settingsProvider);
        }

        public bool AutoRefresh { get; set; }
        
        public bool BodyTabSelected { get; set; }

        public IShellView View { get; private set; }

        public IMessagePropertiesViewModel MessageProperties { get; private set; }

        public IQueueExplorerViewModel QueueExplorer { get; private set; }

        public IEndpointExplorerViewModel EndpointExplorer { get; private set; }

        public IMessageListViewModel Messages { get; private set; }

        public IMessageFlowViewModel MessageFlow { get; private set; }

        public IMessageBodyViewModel MessageBody { get; private set; }

        public IMessageHeadersViewModel MessageHeaders { get; private set; }

        public ISagaWindowViewModel SagaWindow { get; private set; }

        public IStatusBarManager StatusBarManager { get; private set; }

        public ILogWindowViewModel LogWindow { get; private set; }

        public ExplorerItem SelectedExplorerItem { get; private set; }

        public void OnSelectedTabbedViewChanged(object view)
        {
            
        }

        public bool WorkInProgress
        {
            get { return workCounter > 0; }
        }

        public void ShutDown()
        {
            appCommander.ShutdownImmediately();
        }

        public void About()
        {
            windowManager.ShowDialog<AboutViewModel>();
        }

        public void Help()
        {
            Process.Start(@"http://docs.particular.net/");
        }

        public void Options()
        {
            windowManager.ShowDialog<OptionsViewModel>();
        }

        [AutoCheckAvailability]
        public void ConnectToMessageQueue()
        {
            var machineViewModel = screenFactory.CreateScreen<IConnectToMachineViewModel>();
            var result = windowManager.ShowDialog(machineViewModel);

            if(result.GetValueOrDefault(false))
            {
                QueueExplorer.ConnectToQueue(machineViewModel.ComputerName);
            }
        }

        [AutoCheckAvailability]
        public async void ConnectToServiceControl()
        {
            var connectionViewModel = screenFactory.CreateScreen<ServiceControlConnectionViewModel>();
            var result = windowManager.ShowDialog(connectionViewModel);

            if (result.GetValueOrDefault(false))
            {
                await EndpointExplorer.ConnectToService(connectionViewModel.ServiceUrl);
                eventAggregator.Publish(new WorkFinished("Connected to ServiceControl Version {0}", connectionViewModel.Version));
            }
        }

        [AutoCheckAvailability]
        public void DeleteSelectedMessages()
        {
        }

        [AutoCheckAvailability]
        public void PurgeCurrentQueue()
        {
        }

        [AutoCheckAvailability]
        public void DeleteCurrentQueue()
        {
            QueueExplorer.DeleteSelectedQueue();
        }

        [AutoCheckAvailability]
        public async void RefreshAll()
        {
            await EndpointExplorer.RefreshData();
            await QueueExplorer.RefreshData();
            await Messages.RefreshMessages();
            await SagaWindow.RefreshSaga();
        }

        [AutoCheckAvailability]
        public void ImportMessage()
        {
            throw new NotImplementedException("This feature is not yet implemented.");
        }

        [AutoCheckAvailability]
        public void ExportMessage()
        {
            throw new NotImplementedException("This feature is not yet implemented.");
        }

        [AutoCheckAvailability]
        public async Task CreateQueue()
        {
            var screen = screenFactory.CreateScreen<IQueueCreationViewModel>();
            var result = windowManager.ShowDialog(screen);

            if(result.GetValueOrDefault(false))
            {
                await QueueExplorer.RefreshData();
            }
        }

        [AutoCheckAvailability]
        public void CreateMessage()
        {
            throw new NotImplementedException("This feature is not yet implemented.");
        }

        public void Register()
        {
            windowManager.ShowDialog<ILicenseRegistrationViewModel>();
            DisplayRegistrationStatus();
        }

        public void OnAutoRefreshChanged()
        {
            refreshTimer.IsEnabled = AutoRefresh;
        }

        public bool CanCreateMessage
        {
            get { return QueueExplorer.SelectedQueue != null && !WorkInProgress; }
        }

        public bool CanRefreshQueues
        {
            get { return !WorkInProgress; }
        }

        public bool CanPurgeCurrentQueue
        {
            get
            {
                return false;
            }
        }

        public bool CanDeleteCurrentQueue
        {
            get
            {
                return false;
            }
        }

        public bool CanDeleteSelectedMessages
        {
            get
            {
                return false;
            }
        }

        public bool CanCreateQueue
        {
            get
            {
                return !QueueExplorer.ConnectedToAddress.IsEmpty() &&
                       !WorkInProgress &&
                       SelectedExplorerItem.IsQueueExplorerSelected();
            }
        }

        public int SelectedMessageTabItem { get; set; }

        public bool CanConnectToMachine
        {
            get { return !WorkInProgress || AutoRefresh; }
        }

        public bool CanConnectToServiceControl
        {
            get { return !WorkInProgress || AutoRefresh; }
        }

        public bool CanExportMessage
        {
            get { return false; }
        }

        public bool CanImportMessage
        {
            get { return false; }
        }
        
        private void InitializeIdleTimer()
        {
            idleTimer = new DispatcherTimer(DispatcherPriority.Loaded) {Interval = TimeSpan.FromSeconds(10)};
            idleTimer.Tick += (s, e) => OnApplicationIdle();
            idleTimer.Start();
        }

        private void InitializeAutoRefreshTimer()
        {
            var appSetting = settingsProvider.GetSettings<ProfilerSettings>();
            var startupTime = comandLineArgParser.ParsedOptions.ShouldAutoRefresh ? comandLineArgParser.ParsedOptions.AutoRefreshRate : appSetting.AutoRefreshTimer;

            refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(startupTime) };
            refreshTimer.Tick += (s, e) => OnAutoRefreshing();

            AutoRefresh = comandLineArgParser.ParsedOptions.ShouldAutoRefresh;
        }

        internal void OnApplicationIdle()
        {
            if (idleTimer != null)
                idleTimer.Stop();

            ValidateCommandLineArgs();
            ValidateLicense();
        }

        internal void OnAutoRefreshing()
        {
            if (!AutoRefresh || WorkInProgress)
                return;

            RefreshAll();
        }

        public void OnBodyTabSelectedChanged()
        {
            eventAggregator.Publish(new BodyTabSelectionChanged(BodyTabSelected));
        }

        public string AutoRefreshTooltip
        {
            get
            {
                var appSetting = settingsProvider.GetSettings<ProfilerSettings>();
                return string.Format("Automatically update the display every {0} seconds", appSetting.AutoRefreshTimer);
            }
        }

        private void ValidateCommandLineArgs()
        {
            if (comandLineArgParser.HasUnsupportedKeys)
            {
                windowManager.ShowMessageBox("Application was invoked with unsupported arguments.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                appCommander.ShutdownImmediately();
            }
        }
        
        private void ValidateLicense()
        {
            if (licenseManager.IsLicenseExpired())
            {
                RegisterLicense();
            }

            DisplayRegistrationStatus();
        }

        private void DisplayRegistrationStatus()
        {
            var license = licenseManager.CurrentLicense;
            
            if (license == null)
            {
                return;
            }
            if (license.IsCommercialLicense)
            {
                StatusBarManager.SetRegistrationInfo("{0} license, registered to '{1}'",license.LicenseType,license.RegisteredTo);
            }
            else
            {
                StatusBarManager.SetRegistrationInfo("Trial license: {0} left", ("day").PluralizeWord(licenseManager.GetRemainingTrialDays()));
            }
        }

        private void RegisterLicense()
        {
            var model = screenFactory.CreateScreen<ILicenseRegistrationViewModel>();
            var result = windowManager.ShowDialog(model);

            if (!result.GetValueOrDefault(false))
            {
                ShutDown();
            }
        }

        private void NotifyPropertiesChanged()
        {
            NotifyOfPropertyChange(() => WorkInProgress);
            NotifyOfPropertyChange(() => CanConnectToMachine);
            NotifyOfPropertyChange(() => CanConnectToServiceControl);
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

        public void Handle(WorkStarted @event)
        {
            workCounter++;
            NotifyPropertiesChanged();
        }

        public void Handle(WorkFinished @event)
        {
            if (workCounter <= 0) 
                return;

            workCounter--;
            NotifyPropertiesChanged();
        }

        public void Handle(SelectedExplorerItemChanged @event)
        {
            SelectedExplorerItem = @event.SelectedExplorerItem;
        }

        public void Handle(SwitchToMessageBody @event)
        {
            View.SelectTab("MessageBody");
        }

        public virtual void Handle(SwitchToSagaWindow @event)
        {
            View.SelectTab("SagaWindow");
        }

        public virtual void Handle(SwitchToFlowWindow @event)
        {
            View.SelectTab("MessageFlow");
        }

    }
}