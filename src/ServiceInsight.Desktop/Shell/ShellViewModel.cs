namespace Particular.ServiceInsight.Desktop.Shell
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
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

    public class ShellViewModel : Conductor<IScreen>.Collection.AllActive, 
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
        ICommandLineArgParser comandLineArgParser;
        int workCounter;
        DispatcherTimer refreshTimer;
        DispatcherTimer idleTimer;
        
        public ShellViewModel(
            IAppCommands appCommander,
            ScreenFactory screenFactory,
            IWindowManagerEx windowManager,
            IEndpointExplorerViewModel endpointExplorer,
            IMessageListViewModel messages,
            StatusBarManager statusBarManager,
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

        public void ResetLayout()
        {
            View.OnResetLayout(settingsProvider);
        }

        public bool AutoRefresh { get; set; }
        
        public bool BodyTabSelected { get; set; }

        public IShellView View { get; private set; }

        public IMessagePropertiesViewModel MessageProperties { get; private set; }

        public IEndpointExplorerViewModel EndpointExplorer { get; private set; }

        public IMessageListViewModel Messages { get; private set; }

        public IMessageFlowViewModel MessageFlow { get; private set; }

        public IMessageBodyViewModel MessageBody { get; private set; }

        public IMessageHeadersViewModel MessageHeaders { get; private set; }

        public ISagaWindowViewModel SagaWindow { get; private set; }

        public StatusBarManager StatusBarManager { get; private set; }

        public ILogWindowViewModel LogWindow { get; private set; }

        public ExplorerItem SelectedExplorerItem { get; private set; }

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
        public async void RefreshAll()
        {
            await EndpointExplorer.RefreshData();
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
            idleTimer = new DispatcherTimer(DispatcherPriority.Loaded) {Interval = TimeSpan.FromSeconds(10)};
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
                StatusBarManager.SetRegistrationInfo("{0} license, registered to '{1}'",license.LicenseType,license.RegisteredTo);
            }
            else
            {
                StatusBarManager.SetRegistrationInfo("Trial license: {0} left", ("day").PluralizeWord(licenseManager.GetRemainingTrialDays()));
            }
        }

        void RegisterLicense()
        {
            var model = screenFactory.CreateScreen<ILicenseRegistrationViewModel>();
            var result = windowManager.ShowDialog(model);

            if (!result.GetValueOrDefault(false))
            {
                ShutDown();
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