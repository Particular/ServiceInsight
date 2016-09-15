namespace ServiceInsight.Shell
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Explorer;
    using Explorer.EndpointExplorer;
    using ExtensionMethods;
    using Framework;
    using Framework.Rx;
    using global::ServiceInsight.SequenceDiagram;
    using LogWindow;
    using MessageFlow;
    using MessageHeaders;
    using MessageList;
    using MessageProperties;
    using MessageViewers;
    using Options;
    using Pirac;
    using Saga;
    using ServiceControl;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.Licensing;
    using ServiceInsight.Framework.Settings;
    using ServiceInsight.Framework.UI.ScreenManager;
    using Settings;
    using Startup;
    using IScreen = Caliburn.Micro.IScreen;

    public class ShellViewModel : RxConductor<IScreen>.Collection.AllActive, IWorkTracker
    {
        internal const string UnlicensedStatusMessage = "Trial license: {0} left";

        IAppCommands appCommander;
        IWindowManagerEx windowManager;
        IRxEventAggregator eventAggregator;
        AppLicenseManager licenseManager;
        ISettingsProvider settingsProvider;
        CommandLineArgParser comandLineArgParser;
        int workCounter;
        DispatcherTimer refreshTimer;
        DispatcherTimer idleTimer;
        Func<ServiceControlConnectionViewModel> serviceControlConnection;
        Func<LicenseRegistrationViewModel> licenceRegistration;
        ServiceControlConnectionProvider connectionProvider;
        IServiceControl serviceControl;
        IRxServiceControl rxServiceControl;

        public ShellViewModel(
            IAppCommands appCommander,
            IWindowManagerEx windowManager,
            EndpointExplorerViewModel endpointExplorer,
            MessageListViewModel messages,
            Func<ServiceControlConnectionViewModel> serviceControlConnection,
            Func<LicenseRegistrationViewModel> licenceRegistration,
            StatusBarManager statusBarManager,
            IRxEventAggregator eventAggregator,
            AppLicenseManager licenseManager,
            MessageFlowViewModel messageFlow,
            SagaWindowViewModel sagaWindow,
            MessageBodyViewModel messageBodyViewer,
            MessageHeadersViewModel messageHeadersViewer,
            SequenceDiagramViewModel sequenceDiagramViewer,
            ISettingsProvider settingsProvider,
            ServiceControlConnectionProvider connectionProvider,
            IServiceControl serviceControl,
            IRxServiceControl rxServiceControl,
            MessagePropertiesViewModel messageProperties,
            LogWindowViewModel logWindow,
            NetworkOperations networkOperations,
            CommandLineArgParser comandLineArgParser)
        {
            this.appCommander = appCommander;
            this.windowManager = windowManager;
            this.eventAggregator = eventAggregator;
            this.licenseManager = licenseManager;
            this.settingsProvider = settingsProvider;
            this.comandLineArgParser = comandLineArgParser;
            this.serviceControlConnection = serviceControlConnection;
            this.licenceRegistration = licenceRegistration;
            this.connectionProvider = connectionProvider;
            this.serviceControl = serviceControl;
            this.rxServiceControl = rxServiceControl;
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

            ShutDownCommand = Command.Create(appCommander.ShutdownImmediately);
            AboutCommand = Command.Create(() => this.windowManager.ShowDialog<AboutViewModel>());
            HelpCommand = Command.Create(() => networkOperations.Browse(@"http://docs.particular.net/"));
            ConnectToServiceControlCommand = Command.Create(ConnectToServiceControl, () => CanConnectToServiceControl);

            RefreshAllCommand = Command.CreateAsync(() => RefreshAll(true));

            RegisterCommand = Command.Create(() =>
            {
                this.windowManager.ShowDialog<LicenseRegistrationViewModel>();
                DisplayRegistrationStatus();
            });

            ResetLayoutCommand = Command.Create(() => View.OnResetLayout(settingsProvider));

            OptionsCommand = Command.Create(() => windowManager.ShowDialog<OptionsViewModel>());

            eventAggregator.GetEvent<WorkStarted>().Subscribe(Handle);
            eventAggregator.GetEvent<WorkFinished>().Subscribe(Handle);
            eventAggregator.GetEvent<SelectedExplorerItemChanged>().Subscribe(Handle);
            eventAggregator.GetEvent<SwitchToMessageBody>().Subscribe(Handle);
            eventAggregator.GetEvent<SwitchToSagaWindow>().Subscribe(Handle);
            eventAggregator.GetEvent<SwitchToFlowWindow>().Subscribe(Handle);
        }

        string GetConfiguredAddress(CommandLineArgParser commandLineParser)
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

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            View = (IShellView)view;

            DisplayName = GetProductName();
            StatusBarManager.Done();
            RestoreLayout();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            var configuredConnection = GetConfiguredAddress(comandLineArgParser);
            var existingConnection = connectionProvider.Url;

            eventAggregator.Publish(new WorkStarted("Trying to connect to ServiceControl"));

            connectionProvider.ConnectTo(configuredConnection);
            if (!serviceControl.IsAlive())
            {
                connectionProvider.ConnectTo(existingConnection);
            }

            eventAggregator.Publish(new WorkFinished());
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            refreshTimer.Stop();
            SaveLayout();
        }

        void SaveLayout()
        {
            if (!comandLineArgParser.ParsedOptions.ResetLayout)
            {
                View.OnSaveLayout(settingsProvider);
            }
        }

        void RestoreLayout()
        {
            if (!comandLineArgParser.ParsedOptions.ResetLayout)
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

        public ICommand ShutDownCommand { get; }

        public ICommand AboutCommand { get; }

        public ICommand HelpCommand { get; }

        public ICommand ConnectToServiceControlCommand { get; }

        public ICommand RefreshAllCommand { get; }

        public ICommand RegisterCommand { get; }

        public ICommand ResetLayoutCommand { get; }

        public ICommand OptionsCommand { get; }

        public void ConnectToServiceControl()
        {
            var connectionViewModel = serviceControlConnection();
            var result = windowManager.ShowDialog(connectionViewModel);

            if (result.GetValueOrDefault(false))
            {
                //EndpointExplorer.ConnectToService(connectionViewModel.ServiceUrl);
                eventAggregator.Publish(new WorkFinished("Connected to ServiceControl Version {0}", connectionViewModel.Version));
            }
        }

        async Task RefreshAll(bool manual)
        {
            if (manual)
            {
                await rxServiceControl.Refresh();
            }
            Messages.RefreshMessages();
            SagaWindow.RefreshSaga();
        }

        public void OnAutoRefreshChanged()
        {
            refreshTimer.IsEnabled = AutoRefresh;

            if (AutoRefresh)
            {
                rxServiceControl.SetRefresh(refreshTimer.Interval);
            }
            else
            {
                rxServiceControl.DisableRefresh();
            }
        }

        public int SelectedMessageTabItem { get; set; }

        public bool CanConnectToServiceControl => !WorkInProgress || AutoRefresh;

        void InitializeIdleTimer()
        {
            idleTimer = new DispatcherTimer(DispatcherPriority.Loaded) { Interval = TimeSpan.FromSeconds(10) };
            idleTimer.Tick += (s, e) => OnApplicationIdle();
            idleTimer.Start();
        }

        void InitializeAutoRefreshTimer()
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
            {
                idleTimer.Stop();
            }

            ValidateCommandLineArgs();
            ValidateLicense();
        }

        internal Task OnAutoRefreshing()
        {
            if (!AutoRefresh || WorkInProgress)
            {
                return Task.FromResult(0);
            }

            return RefreshAll(false);
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

        void ValidateCommandLineArgs()
        {
            if (comandLineArgParser.HasUnsupportedKeys)
            {
                windowManager.ShowMessageBox("Application was invoked with unsupported arguments.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                appCommander.ShutdownImmediately();
            }
        }

        void ValidateLicense()
        {
            if (licenseManager.IsLicenseExpired())
            {
                RegisterLicense();
            }

            DisplayRegistrationStatus();
        }

        void DisplayRegistrationStatus()
        {
            var license = licenseManager.CurrentLicense;

            if (license == null)
            {
                return;
            }
            if (license.IsCommercialLicense)
            {
                StatusBarManager.SetRegistrationInfo("{0} license, registered to '{1}'", license.LicenseType, license.RegisteredTo);
            }
            else
            {
                StatusBarManager.SetRegistrationInfo(UnlicensedStatusMessage, "day".PluralizeWord(licenseManager.GetRemainingTrialDays()));
            }
        }

        void RegisterLicense()
        {
            var model = licenceRegistration();
            var result = windowManager.ShowDialog(model);

            if (!result.GetValueOrDefault(false))
            {
                appCommander.ShutdownImmediately();
            }
        }

        void NotifyPropertiesChanged()
        {
            NotifyOfPropertyChange(() => WorkInProgress);
            NotifyOfPropertyChange(() => CanConnectToServiceControl);
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

        void Handle(SwitchToMessageBody @event)
        {
            View.SelectTab("MessageBody");
        }

        void Handle(SwitchToSagaWindow @event)
        {
            View.SelectTab("SagaWindow");
        }

        void Handle(SwitchToFlowWindow @event)
        {
            View.SelectTab("MessageFlow");
        }
    }
}