namespace ServiceInsight.Tests
{
    using System;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Explorer.EndpointExplorer;
    using LogWindow;
    using MessageFlow;
    using MessageHeaders;
    using MessageList;
    using MessageProperties;
    using MessageViewers;
    using Models;
    using NSubstitute;
    using NUnit.Framework;
    using Particular.Licensing;
    using Saga;
    using SequenceDiagram;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.Licensing;
    using ServiceInsight.Framework.Settings;
    using ServiceInsight.Framework.UI.ScreenManager;
    using ServiceInsight.Startup;
    using Settings;
    using Shell;
    using Shouldly;

    public interface IShellViewStub : IShellView
    {
        bool IsOpen { get; set; }

        void Close();
    }

    [TestFixture]
    public class ShellViewModelTests
    {
        ShellViewModel shell;
        ServiceInsightWindowManager windowManager;
        EndpointExplorerViewModel endpointExplorer;
        MessageListViewModel messageList;
        MessageFlowViewModel messageFlow;
        SagaWindowViewModel sagaWindow;
        IVersionUpdateChecker versionUpdateChecker;
        IEventAggregator eventAggregator;
        IWorkNotifier workNotifier;
        StatusBarManager statusbarManager;
        MessageBodyViewModel messageBodyView;
        MessageHeadersViewModel headerView;
        SequenceDiagramViewModel sequenceDiagramView;
        ISettingsProvider settingsProvider;
        AppLicenseManager licenseManager;
        IShellViewStub view;
        MessagePropertiesViewModel messageProperties;
        LogWindowViewModel logWindow;
        IAppCommands app;
        CommandLineArgParser commandLineArgParser;
        LicenseStatusBar licenseStatusBar;

        [SetUp]
        public void TestInitialize()
        {
            windowManager = Substitute.For<ServiceInsightWindowManager>();
            endpointExplorer = Substitute.For<EndpointExplorerViewModel>();
            messageList = Substitute.For<MessageListViewModel>();
            licenseStatusBar = Substitute.For<LicenseStatusBar>();
            statusbarManager = new StatusBarManager(licenseStatusBar);
            eventAggregator = Substitute.For<IEventAggregator>();
            workNotifier = Substitute.For<IWorkNotifier>();
            messageFlow = Substitute.For<MessageFlowViewModel>();
            sagaWindow = Substitute.For<SagaWindowViewModel>();
            messageBodyView = Substitute.For<MessageBodyViewModel>();
            messageProperties = Substitute.For<MessagePropertiesViewModel>();
            view = Substitute.For<IShellViewStub>();
            headerView = Substitute.For<MessageHeadersViewModel>();
            sequenceDiagramView = Substitute.For<SequenceDiagramViewModel>();
            settingsProvider = Substitute.For<ISettingsProvider>();
            licenseManager = Substitute.For<AppLicenseManager>();
            versionUpdateChecker = Substitute.For<IVersionUpdateChecker>();
            logWindow = Substitute.For<LogWindowViewModel>();
            settingsProvider.GetSettings<ProfilerSettings>().Returns(DefaultAppSetting());
            app = Substitute.For<IAppCommands>();
            commandLineArgParser = MockEmptyStartupOptions();

            shell = new ShellViewModel(
                        app,
                        windowManager,
                        Substitute.For<IApplicationVersionService>(),
                        logWindow,
                        endpointExplorer,
                        messageList,
                        () => Substitute.For<ServiceControlConnectionViewModel>(),
                        () => Substitute.For<LicenseMessageBoxViewModel>(),
                        statusbarManager,
                        eventAggregator,
                        workNotifier,
                        licenseManager,
                        messageFlow,
                        sagaWindow,
                        headerView,
                        sequenceDiagramView,
                        settingsProvider,
                        versionUpdateChecker,
                        messageProperties,
                        commandLineArgParser,
                        messageBodyView);

            ((IViewAware)shell).AttachView(view);
        }

        CommandLineArgParser MockEmptyStartupOptions()
        {
            var parser = Substitute.For<CommandLineArgParser>();

            parser.ParsedOptions.Returns(new CommandLineOptions());

            return parser;
        }

        [Test]
        public void Should_reload_stored_layout()
        {
            view.Received().OnRestoreLayout(settingsProvider);
        }

        [Test]
        public void Should_still_report_work_in_progress_when_only_part_of_the_work_is_finished()
        {
            shell.Handle(new WorkStarted("Some Work"));
            shell.Handle(new WorkStarted("Some Other Work"));

            shell.Handle(new WorkFinished());

            shell.WorkInProgress.ShouldBe(true);
        }

        [Test]
        public void Should_finish_all_the_works_in_progress_when_the_work_is_finished()
        {
            shell.Handle(new WorkStarted());

            shell.Handle(new WorkFinished()); //TODO: Why two?
            shell.Handle(new WorkFinished());

            shell.WorkInProgress.ShouldBe(false);
        }

        [Test]
        public void Deactivating_shell_saves_layout()
        {
            ((IScreen)shell).Activate();

            ((IScreen)shell).Deactivate(true);

            view.Received().OnSaveLayout(settingsProvider);
        }

        [Test]
        public void Deactivating_shell_saves_part_layouts()
        {
            ((IScreen)shell).Activate();

            ((IScreen)shell).Deactivate(true);

            messageList.Received().OnSavePartLayout();
        }

        [Test]
        public void Should_track_selected_explorer()
        {
            var serviceControl = new ServiceControlExplorerItem("http://localhost:3333/api");
            var selected = new AuditEndpointExplorerItem(serviceControl, new Endpoint { Name = "Sales" });

            shell.Handle(new SelectedExplorerItemChanged(selected));

            shell.SelectedExplorerItem.ShouldNotBe(null);
            shell.SelectedExplorerItem.ShouldBeSameAs(selected);
        }

        [Test]
        [Ignore("Test prevents NUnit AppDomain from unloading")]
        public async Task Should_validate_trial_license()
        {
            const string RegisteredUser = "John Doe";
            const string LicenseType = "Trial";
            const int RemainingDays = 5;

            var issuedLicense = new License
            {
                LicenseType = LicenseType,
#pragma warning disable PS0023 // DateTime.UtcNow or DateTimeOffset.UtcNow should be used instead of DateTime.Now and DateTimeOffset.Now, unless the value is being used for displaying the current date-time in a user's local time zone
                ExpirationDate = DateTime.Now.AddDays(RemainingDays),
#pragma warning restore PS0023 // DateTime.UtcNow or DateTimeOffset.UtcNow should be used instead of DateTime.Now and DateTimeOffset.Now, unless the value is being used for displaying the current date-time in a user's local time zone
                RegisteredTo = RegisteredUser
            };

            licenseManager.CurrentLicense = issuedLicense;
            licenseManager.GetRemainingNonProductionDays().Returns(RemainingDays);

            await shell.OnApplicationIdle();

            licenseStatusBar.Received().SetNonProductionRemainingDays(Arg.Is(RemainingDays));
        }

        [TearDown]
        public void TestCleanup()
        {
            ((IScreen)shell).Deactivate(true);
        }

        static ProfilerSettings DefaultAppSetting() => new ProfilerSettings
        {
            AutoRefreshTimer = 15,
        };
    }
}