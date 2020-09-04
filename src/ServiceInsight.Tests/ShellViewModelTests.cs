﻿namespace ServiceInsight.Tests
{
    using System;
    using Caliburn.Micro;
    using global::ServiceInsight.SequenceDiagram;
    using NSubstitute;
    using NUnit.Framework;
    using Particular.Licensing;
    using ServiceInsight.Explorer.EndpointExplorer;
    using ServiceInsight.Framework;
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
        WindowManagerEx windowManager;
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
            windowManager = Substitute.For<WindowManagerEx>();
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
                        messageBodyView,
                        headerView,
                        sequenceDiagramView,
                        settingsProvider,
                        versionUpdateChecker,
                        messageProperties,
                        logWindow,
                        commandLineArgParser);

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

        public void Should_track_selected_explorer()
        {
            var selected = new AuditEndpointExplorerItem(new Endpoint { Name = "Sales" });

            shell.Handle(new SelectedExplorerItemChanged(selected));

            shell.SelectedExplorerItem.ShouldNotBe(null);
            shell.SelectedExplorerItem.ShouldBeSameAs(selected);
        }

        [Test]
        public void Should_validate_trial_license()
        {
            const string RegisteredUser = "John Doe";
            const string LicenseType = "Trial";
            const int RemainingDays = 5;

            var issuedLicense = new License
            {
                LicenseType = LicenseType,
                ExpirationDate = DateTime.Now.AddDays(RemainingDays),
                RegisteredTo = RegisteredUser
            };

            licenseManager.CurrentLicense = issuedLicense;
            licenseManager.GetRemainingNonProductionDays().Returns(RemainingDays);

            shell.OnApplicationIdle();

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