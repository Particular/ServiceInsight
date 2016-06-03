namespace ServiceInsight.Tests
{
    using System;
    using Caliburn.Micro;
    using global::ServiceInsight.SequenceDiagram;
    using NSubstitute;
    using NUnit.Framework;
    using Particular.Licensing;
    using ServiceInsight.Explorer.EndpointExplorer;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.Licensing;
    using ServiceInsight.Framework.Settings;
    using ServiceInsight.Framework.UI.ScreenManager;
    using ServiceInsight.LogWindow;
    using ServiceInsight.MessageFlow;
    using ServiceInsight.MessageHeaders;
    using ServiceInsight.MessageList;
    using ServiceInsight.MessageProperties;
    using ServiceInsight.MessageViewers;
    using ServiceInsight.Models;
    using ServiceInsight.Saga;
    using ServiceInsight.Settings;
    using ServiceInsight.Shell;
    using ServiceInsight.Startup;
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
        WindowManagerEx WindowManager;
        EndpointExplorerViewModel EndpointExplorer;
        MessageListViewModel MessageList;
        MessageFlowViewModel MessageFlow;
        SagaWindowViewModel SagaWindow;
        IEventAggregator EventAggregator;
        StatusBarManager StatusbarManager;
        MessageBodyViewModel MessageBodyView;
        MessageHeadersViewModel HeaderView;
        SequenceDiagramViewModel SequenceDiagramView;
        ISettingsProvider SettingsProvider;
        AppLicenseManager LicenseManager;
        IShellViewStub View;
        MessagePropertiesViewModel MessageProperties;
        LogWindowViewModel LogWindow;
        IAppCommands App;
        CommandLineArgParser CommandLineArgParser;

        [SetUp]
        public void TestInitialize()
        {
            WindowManager = Substitute.For<WindowManagerEx>();
            EndpointExplorer = Substitute.For<EndpointExplorerViewModel>();
            MessageList = Substitute.For<MessageListViewModel>();
            StatusbarManager = Substitute.For<StatusBarManager>();
            EventAggregator = Substitute.For<IEventAggregator>();
            MessageFlow = Substitute.For<MessageFlowViewModel>();
            SagaWindow = Substitute.For<SagaWindowViewModel>();
            MessageBodyView = Substitute.For<MessageBodyViewModel>();
            MessageProperties = Substitute.For<MessagePropertiesViewModel>();
            View = Substitute.For<IShellViewStub>();
            HeaderView = Substitute.For<MessageHeadersViewModel>();
            SequenceDiagramView = Substitute.For<SequenceDiagramViewModel>();
            SettingsProvider = Substitute.For<ISettingsProvider>();
            LicenseManager = Substitute.For<AppLicenseManager>();
            LogWindow = Substitute.For<LogWindowViewModel>();
            SettingsProvider.GetSettings<ProfilerSettings>().Returns(DefaultAppSetting());
            App = Substitute.For<IAppCommands>();
            CommandLineArgParser = MockEmptyStartupOptions();

            shell = new ShellViewModel(
                        App,
                        WindowManager,
                        EndpointExplorer,
                        MessageList,
                        () => Substitute.For<ServiceControlConnectionViewModel>(),
                        () => Substitute.For<LicenseRegistrationViewModel>(),
                        StatusbarManager,
                        EventAggregator,
                        LicenseManager,
                        MessageFlow,
                        SagaWindow,
                        MessageBodyView,
                        HeaderView,
                        SequenceDiagramView,
                        SettingsProvider,
                        MessageProperties,
                        LogWindow,
                        CommandLineArgParser);

            ((IViewAware)shell).AttachView(View);
        }

        CommandLineArgParser MockEmptyStartupOptions()
        {
            var parser = Substitute.For<CommandLineArgParser>();

            parser.ParsedOptions.Returns(new CommandLineOptions());

            return parser;
        }

        [Test]
        public void should_reload_stored_layout()
        {
            View.Received().OnRestoreLayout(SettingsProvider);
        }

        [Test]
        public void should_still_report_work_in_progress_when_only_part_of_the_work_is_finished()
        {
            shell.Handle(new WorkStarted("Some Work"));
            shell.Handle(new WorkStarted("Some Other Work"));

            shell.Handle(new WorkFinished());

            shell.WorkInProgress.ShouldBe(true);
        }

        [Test]
        public void should_finish_all_the_works_in_progress_when_the_work_is_finished()
        {
            shell.Handle(new WorkStarted());

            shell.Handle(new WorkFinished()); //TODO: Why two?
            shell.Handle(new WorkFinished());

            shell.WorkInProgress.ShouldBe(false);
        }

        [Test]
        public void deactivating_shell_saves_layout()
        {
            ((IScreen)shell).Activate();

            ((IScreen)shell).Deactivate(true);

            View.Received().OnSaveLayout(SettingsProvider);
        }

        public void should_track_selected_explorer()
        {
            var selected = new AuditEndpointExplorerItem(new Endpoint { Name = "Sales" });

            shell.Handle(new SelectedExplorerItemChanged(selected));

            shell.SelectedExplorerItem.ShouldNotBe(null);
            shell.SelectedExplorerItem.ShouldBeSameAs(selected);
        }

        [Test]
        public void should_validate_trial_license()
        {
            const string RegisteredUser = "John Doe";
            const string LicenseType = "Trial";
            const int NumberOfDaysRemainingFromTrial = 5;

            var issuedLicense = new License
            {
                LicenseType = LicenseType,
                ExpirationDate = DateTime.Now.AddDays(NumberOfDaysRemainingFromTrial),
                RegisteredTo = RegisteredUser
            };

            LicenseManager.CurrentLicense = issuedLicense;
            LicenseManager.GetRemainingTrialDays().Returns(NumberOfDaysRemainingFromTrial);

            shell.OnApplicationIdle();

            StatusbarManager.Received().SetRegistrationInfo(Arg.Is(ShellViewModel.UnlicensedStatusMessage), Arg.Is("5 days"));
        }

        [TearDown]
        public void TestCleanup()
        {
            ((IScreen)shell).Deactivate(true);
        }

        static ProfilerSettings DefaultAppSetting()
        {
            return new ProfilerSettings
            {
                AutoRefreshTimer = 15,
            };
        }
    }
}