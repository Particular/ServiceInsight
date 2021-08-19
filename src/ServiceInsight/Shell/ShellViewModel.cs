﻿namespace ServiceInsight.Shell
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Caliburn.Micro;
    using Explorer;
    using Explorer.EndpointExplorer;
    using ExtensionMethods;
    using Framework;
    using Framework.Events;
    using Framework.Licensing;
    using Framework.Rx;
    using Framework.Settings;
    using Framework.UI.ScreenManager;
    using LogWindow;
    using MessageFlow;
    using MessageHeaders;
    using MessageList;
    using MessageProperties;
    using MessageViewers;
    using Options;
    using Saga;
    using SequenceDiagram;
    using Settings;
    using Startup;

    public class ShellViewModel : RxConductor<IScreen>.Collection.AllActive,
        IHandle<WorkStarted>,
        IHandle<WorkFinished>,
        IHandle<SelectedExplorerItemChanged>,
        IHandle<SwitchToMessageBody>,
        IHandle<SwitchToSagaWindow>,
        IHandle<SwitchToFlowWindow>,
        IHandle<LicenseUpdated>,
        IWorkTracker
    {
        readonly IAppCommands appCommander;
        readonly IServiceInsightWindowManager windowManager;
        readonly IEventAggregator eventAggregator;
        readonly IWorkNotifier workNotifier;
        readonly IVersionUpdateChecker versionUpdateChecker;
        readonly AppLicenseManager licenseManager;
        readonly ISettingsProvider settingsProvider;
        readonly CommandLineArgParser commandLineArgParser;
        readonly Func<ServiceControlConnectionViewModel> serviceControlConnection;
        readonly Func<LicenseMessageBoxViewModel> licenseMessageBoxViewModel;

        int workCounter;
        DispatcherTimer refreshTimer;
        DispatcherTimer idleTimer;

        public ShellViewModel(
            IAppCommands appCommander,
            IServiceInsightWindowManager windowManager,
            IApplicationVersionService applicationVersionService,
            LogWindowViewModel logWindow,
            EndpointExplorerViewModel endpointExplorer,
            MessageListViewModel messages,
            Func<ServiceControlConnectionViewModel> serviceControlConnection,
            Func<LicenseMessageBoxViewModel> licenseMessageBoxViewModel,
            StatusBarManager statusBarManager,
            IEventAggregator eventAggregator,
            IWorkNotifier workNotifier,
            AppLicenseManager licenseManager,
            MessageFlowViewModel messageFlow,
            SagaWindowViewModel sagaWindow,
            MessageHeadersViewModel messageHeadersViewer,
            SequenceDiagramViewModel sequenceDiagramViewer,
            ISettingsProvider settingsProvider,
            IVersionUpdateChecker versionUpdateChecker,
            MessagePropertiesViewModel messageProperties,
            CommandLineArgParser commandLineArgParser,
            MessageBodyViewModel messageBodyViewer)
        {
            this.appCommander = appCommander;
            this.windowManager = windowManager;
            this.eventAggregator = eventAggregator;
            this.workNotifier = workNotifier;
            this.licenseManager = licenseManager;
            this.settingsProvider = settingsProvider;
            this.commandLineArgParser = commandLineArgParser;
            this.serviceControlConnection = serviceControlConnection;
            this.licenseMessageBoxViewModel = licenseMessageBoxViewModel;
            this.versionUpdateChecker = versionUpdateChecker;
            MessageProperties = messageProperties;
            MessageFlow = messageFlow;
            SagaWindow = sagaWindow;
            StatusBarManager = statusBarManager;
            EndpointExplorer = endpointExplorer;
            MessageHeaders = messageHeadersViewer;
            MessageBody = messageBodyViewer;
            SequenceDiagram = sequenceDiagramViewer;
            Messages = messages;
            LogWindow = logWindow;

            Items.Add(endpointExplorer);
            Items.Add(messages);
            Items.Add(messageHeadersViewer);
            Items.Add(messageBodyViewer);
            Items.Add(messageFlow);

            InitializeAutoRefreshTimer();
            InitializeIdleTimer();

            ShutDownCommand = Command.Create(() => this.appCommander.ShutdownImmediately());
            AboutCommand = Command.Create(() => this.windowManager.ShowDialog<AboutViewModel>());
            HelpCommand = Command.Create(() => Process.Start(@"http://docs.particular.net/serviceinsight"));
            ConnectToServiceControlCommand = Command.CreateAsync(ConnectToServiceControl);
            DisconnectServiceControlCommand = Command.CreateAsync(DisconnectServiceControl);
            ProvideFeedbackCommand = Command.Create(() => Process.Start($"https://github.com/Particular/ServiceInsight/issues/new?title=Feedback%20for%20ServiceInsight%20{applicationVersionService.GetVersion()}%20({applicationVersionService.GetCommitHash()})&body=Your%20feedback..."));
            RefreshAllCommand = Command.CreateAsync(RefreshAll);

            RegisterCommand = Command.Create(() => windowManager.ShowDialog<ManageLicenseViewModel>());

            ResetLayoutCommand = Command.Create(() => View.OnResetLayout(settingsProvider));

            OptionsCommand = Command.Create(() => windowManager.ShowDialog<OptionsViewModel>());

            NewVersionIsAvailableCommand = Command.Create(() => Process.Start(@"https://github.com/Particular/ServiceInsight/releases"));
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            View = (IShellView)view;

            DisplayName = GetProductName();
            StatusBarManager.Done();
            RestoreLayout();
        }

        protected override void OnDeactivate(bool close)
        {
            refreshTimer.Stop();
            SaveLayout();
            base.OnDeactivate(close);
        }

        void SaveLayout()
        {
            if (!commandLineArgParser.ParsedOptions.ResetLayout)
            {
                foreach (var screen in Items.OfType<IPersistPartLayout>())
                {
                    screen.OnSavePartLayout();
                }

                View.OnSaveLayout(settingsProvider);
            }
        }

        void RestoreLayout()
        {
            if (!commandLineArgParser.ParsedOptions.ResetLayout)
            {
                View.OnRestoreLayout(settingsProvider);
            }
        }

        public bool AutoRefresh { get; set; }

        public bool BodyTabSelected { get; set; }

        public IShellView View { get; private set; }

        public MessagePropertiesViewModel MessageProperties { get; }

        public EndpointExplorerViewModel EndpointExplorer { get; }

        public MessageListViewModel Messages { get; }

        public MessageFlowViewModel MessageFlow { get; }

        public MessageBodyViewModel MessageBody { get; }

        public MessageHeadersViewModel MessageHeaders { get; }

        public SequenceDiagramViewModel SequenceDiagram { get; }

        public SagaWindowViewModel SagaWindow { get; }

        public StatusBarManager StatusBarManager { get; }

        public LogWindowViewModel LogWindow { get; }

        public ExplorerItem SelectedExplorerItem { get; private set; }

        public bool WorkInProgress => workCounter > 0;

        public bool NewVersionIsAvailable { get; private set; }

        public ICommand ShutDownCommand { get; }

        public ICommand AboutCommand { get; }

        public ICommand HelpCommand { get; }

        public ICommand ConnectToServiceControlCommand { get; }

        public ICommand DisconnectServiceControlCommand { get; }

        public ICommand RefreshAllCommand { get; }

        public ICommand RegisterCommand { get; }

        public ICommand ResetLayoutCommand { get; }

        public ICommand OptionsCommand { get; }

        public ICommand NewVersionIsAvailableCommand { get; }

        public ICommand ProvideFeedbackCommand { get; }

        public async Task ConnectToServiceControl()
        {
            if (ShouldConnectToServiceControl)
            {
                var connectionViewModel = serviceControlConnection();
                var result = windowManager.ShowDialog(connectionViewModel);

                if (result.GetValueOrDefault(false))
                {
                    using (workNotifier.NotifyOfWork("Connecting...", $"Connected to ServiceControl Version {connectionViewModel.Version}"))
                    {
                        await EndpointExplorer.ConnectToService(connectionViewModel.ServiceUrl);
                    }
                }
            }
        }

        public async Task DisconnectServiceControl()
        {
            if (ShouldDisconnectServiceControl)
            {
                await EndpointExplorer.DisconnectSelectedServiceControl();
            }
        }

        async Task AutoRefreshAll()
        {
            await EndpointExplorer.RefreshAllEndpoints();
            await Messages.RefreshMessages();
            await SagaWindow.RefreshSaga();
        }

        async Task RefreshAll()
        {
            await EndpointExplorer.RefreshSelectedEndpoint();
            await Messages.RefreshMessages();
            await SagaWindow.RefreshSaga();
        }

        public void OnAutoRefreshChanged()
        {
            refreshTimer.IsEnabled = AutoRefresh;
        }

        public int SelectedMessageTabItem { get; set; }

        public bool ShouldConnectToServiceControl => !WorkInProgress || AutoRefresh;

        public bool ShouldDisconnectServiceControl => !WorkInProgress && EndpointExplorer != null &&
                                                   EndpointExplorer.HasSelectedServiceControl;

        void InitializeIdleTimer()
        {
            idleTimer = new DispatcherTimer(DispatcherPriority.Loaded) { Interval = TimeSpan.FromSeconds(10) };
            idleTimer.Tick += async (s, e) => await OnApplicationIdle();
            idleTimer.Start();
        }

        void InitializeAutoRefreshTimer()
        {
            var appSetting = settingsProvider.GetSettings<ProfilerSettings>();
            var startupTime = commandLineArgParser.ParsedOptions.ShouldAutoRefresh ? commandLineArgParser.ParsedOptions.AutoRefreshRate : appSetting.AutoRefreshTimer;

            if (refreshTimer == null)
            {
                refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(startupTime) };
                refreshTimer.Tick += async (s, e) => await OnAutoRefreshing();
            }
            else
            {
                refreshTimer.Interval = TimeSpan.FromSeconds(startupTime);
            }

            AutoRefresh = commandLineArgParser.ParsedOptions.ShouldAutoRefresh;
        }

        internal async Task OnApplicationIdle()
        {
            if (idleTimer != null)
            {
                idleTimer.Stop();
            }

            await StartupConfigChangeListener();
            ValidateCommandLineArgs();
            ValidateLicense();
            CheckForUpdates();
        }

        Task StartupConfigChangeListener()
        {
            return Task.Factory.StartNew(() => StartupConfigListener.Start(eventAggregator, commandLineArgParser));
        }

        internal async Task OnAutoRefreshing()
        {
            if (!AutoRefresh || WorkInProgress)
            {
                return;
            }

            await AutoRefreshAll();
        }

        public void OnBodyTabSelectedChanged()
        {
            eventAggregator.PublishOnUIThread(new BodyTabSelectionChanged(BodyTabSelected));
        }

        public string AutoRefreshTooltip
        {
            get
            {
                var appSetting = settingsProvider.GetSettings<ProfilerSettings>();
                return string.Format("Automatically update the display every {0} seconds", appSetting.AutoRefreshTimer);
            }
        }

        void CheckForUpdates()
        {
            NewVersionIsAvailable = versionUpdateChecker.IsNewVersionAvailable();
        }

        void ValidateCommandLineArgs()
        {
            if (commandLineArgParser.HasUnsupportedKeys)
            {
                windowManager.ShowMessageBox("Application was invoked with unsupported arguments.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                appCommander.ShutdownImmediately();
            }
        }

        void ValidateLicense()
        {
            DisplayLicenseStatus(true);
            DisplayRegistrationStatus();

            if (licenseManager.IsLicenseExpired())
            {
                RegisterLicense();
            }
        }

        void DisplayLicenseStatus(bool appStartCheck)
        {
            var license = licenseManager.CurrentLicense;

            if (license == null)
            {
                return;
            }

            StatusBarManager.LicenseStatus.AppStartCheck = appStartCheck;

            if (license.IsCommercialLicense)
            {
                var upgradeProtectionDays = licenseManager.GetUpgradeProtectionRemainingDays();
                var expirationDays = licenseManager.GetExpirationRemainingDays();

                if (upgradeProtectionDays.HasValue)
                {
                    StatusBarManager.LicenseStatus.SetLicenseUpgradeProtectionDays(upgradeProtectionDays.Value);
                }

                if (expirationDays.HasValue)
                {
                    StatusBarManager.LicenseStatus.SetLicenseRemainingDays(expirationDays.Value);
                }
            }
            else
            {
                StatusBarManager.LicenseStatus.SetNonProductionRemainingDays(licenseManager.GetRemainingNonProductionDays());
            }
        }

        void DisplayRegistrationStatus()
        {
            var license = licenseManager.CurrentLicense;

            if (license == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(license.RegisteredTo) == false)
            {
                StatusBarManager.LicenseStatus.SetRegistrationInfo("{0} license, registered to '{1}'", license.LicenseType, license.RegisteredTo);
            }
            else
            {
                StatusBarManager.LicenseStatus.SetRegistrationInfo("{0} license", license.LicenseType);
            }
        }

        void RegisterLicense()
        {
            var model = licenseMessageBoxViewModel();
            model.DisplayName = StatusBarManager.LicenseStatus.LicenseStatusMessage;
            var result = windowManager.ShowDialog(model);

            if (!result.GetValueOrDefault(false))
            {
                appCommander.ShutdownImmediately();
            }
        }

        void NotifyPropertiesChanged()
        {
            NotifyOfPropertyChange(nameof(WorkInProgress));
        }

        string GetProductName()
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
            {
                return;
            }

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

        public virtual void Handle(LicenseUpdated message)
        {
            DisplayLicenseStatus(false);
            DisplayRegistrationStatus();
        }

        public async Task PostConfigurationUpdate()
        {
            await Messages.SearchBar.PerformCommandLineSearch();
            InitializeAutoRefreshTimer();
        }
    }
}