namespace Particular.ServiceInsight.Desktop.Shell
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Caliburn.Micro;
    using Core.Licensing;
    using Core.Settings;
    using Core.UI.ScreenManager;
    using Events;
    using Explorer;
    using Explorer.EndpointExplorer;
    using ExtensionMethods;
    using Framework.Rx;
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
    using IScreen = Caliburn.Micro.IScreen;

    public class ShellViewModel : RxConductor<IScreen>.Collection.AllActive,
        IDeactivate,
        IHandle<WorkStarted>,
        IHandle<WorkFinished>,
        IHandle<SelectedExplorerItemChanged>,
        IHandle<SwitchToMessageBody>,
        IHandle<SwitchToSagaWindow>,
        IHandle<SwitchToFlowWindow>,
        IWorkTracker
    {
        IAppCommands appCommander;
        ScreenFactory screenFactory;
        IWindowManagerEx windowManager;
        IEventAggregator eventAggregator;
        AppLicenseManager licenseManager;
        ISettingsProvider settingsProvider;
        CommandLineArgParser comandLineArgParser;
        int workCounter;
        DispatcherTimer refreshTimer;
        DispatcherTimer idleTimer;

        public ShellViewModel(
            IAppCommands appCommander,
            ScreenFactory screenFactory,
            IWindowManagerEx windowManager,
            EndpointExplorerViewModel endpointExplorer,
            MessageListViewModel messages,
            StatusBarManager statusBarManager,
            IEventAggregator eventAggregator,
            AppLicenseManager licenseManager,
            MessageFlowViewModel messageFlow,
            SagaWindowViewModel sagaWindow,
            MessageBodyViewModel messageBodyViewer,
            MessageHeadersViewModel messageHeadersViewer,
            ISettingsProvider settingsProvider,
            MessagePropertiesViewModel messageProperties,
            LogWindowViewModel logWindow,
            CommandLineArgParser comandLineArgParser)
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
            EndpointExplorer = endpointExplorer;
            MessageHeaders = messageHeadersViewer;
            MessageBody = messageBodyViewer;
            Messages = messages;
            LogWindow = logWindow;

            Items.Add(endpointExplorer);
            Items.Add(messages);
            Items.Add(messageHeadersViewer);
            Items.Add(messageBodyViewer);
            Items.Add(messageFlow);

            InitializeAutoRefreshTimer();
            InitializeIdleTimer();

            ShutDownCommand = this.CreateCommand(() => this.appCommander.ShutdownImmediately());
            AboutCommand = this.CreateCommand(() => this.windowManager.ShowDialog<AboutViewModel>());
            HelpCommand = this.CreateCommand(() => Process.Start(@"http://docs.particular.net/"));
            ConnectToServiceControlCommand = this.CreateCommand(ConnectToServiceControl, vm => vm.CanConnectToServiceControl);

            RefreshAllCommand = this.CreateCommand(RefreshAll);

            RegisterCommand = this.CreateCommand(() =>
            {
                this.windowManager.ShowDialog<LicenseRegistrationViewModel>();
                DisplayRegistrationStatus();
            });

            ResetLayoutCommand = this.CreateCommand(() => View.OnResetLayout(settingsProvider));

            OptionsCommand = this.CreateCommand(() => windowManager.ShowDialog<OptionsViewModel>());
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
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

        public MessagePropertiesViewModel MessageProperties { get; private set; }

        public EndpointExplorerViewModel EndpointExplorer { get; private set; }

        public MessageListViewModel Messages { get; private set; }

        public MessageFlowViewModel MessageFlow { get; private set; }

        public MessageBodyViewModel MessageBody { get; private set; }

        public MessageHeadersViewModel MessageHeaders { get; private set; }

        public SagaWindowViewModel SagaWindow { get; private set; }

        public StatusBarManager StatusBarManager { get; private set; }

        public LogWindowViewModel LogWindow { get; private set; }

        public ExplorerItem SelectedExplorerItem { get; private set; }

        public bool WorkInProgress
        {
            get { return workCounter > 0; }
        }

        public ICommand ShutDownCommand { get; private set; }

        public ICommand AboutCommand { get; private set; }

        public ICommand HelpCommand { get; private set; }

        public ICommand ConnectToServiceControlCommand { get; private set; }

        public ICommand RefreshAllCommand { get; private set; }

        public ICommand RegisterCommand { get; private set; }

        public ICommand ResetLayoutCommand { get; private set; }

        public ICommand OptionsCommand { get; private set; }

        public void ConnectToServiceControl()
        {
            var connectionViewModel = screenFactory.CreateScreen<ServiceControlConnectionViewModel>();
            var result = windowManager.ShowDialog(connectionViewModel);

            if (result.GetValueOrDefault(false))
            {
                EndpointExplorer.ConnectToService(connectionViewModel.ServiceUrl);
                eventAggregator.Publish(new WorkFinished("Connected to ServiceControl Version {0}", connectionViewModel.Version));
            }
        }

        public void DeleteSelectedMessages()
        {
        }

        void RefreshAll()
        {
            EndpointExplorer.RefreshData();
            Messages.RefreshMessages();
            SagaWindow.RefreshSaga();
        }

        public void ImportMessage()
        {
            throw new NotImplementedException("This feature is not yet implemented.");
        }

        public void ExportMessage()
        {
            throw new NotImplementedException("This feature is not yet implemented.");
        }

        public void CreateMessage()
        {
            throw new NotImplementedException("This feature is not yet implemented.");
        }

        public void OnAutoRefreshChanged()
        {
            refreshTimer.IsEnabled = AutoRefresh;
        }

        public bool CanDeleteSelectedMessages
        {
            get
            {
                return false;
            }
        }

        public int SelectedMessageTabItem { get; set; }

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
                StatusBarManager.SetRegistrationInfo("Trial license: {0} left", ("day").PluralizeWord(licenseManager.GetRemainingTrialDays()));
            }
        }

        void RegisterLicense()
        {
            var model = screenFactory.CreateScreen<LicenseRegistrationViewModel>();
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
            NotifyOfPropertyChange(() => CanDeleteSelectedMessages);
            NotifyOfPropertyChange(() => CanExportMessage);
            NotifyOfPropertyChange(() => CanImportMessage);
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