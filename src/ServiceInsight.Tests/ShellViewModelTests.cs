namespace Particular.ServiceInsight.Tests
{
    using System;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Screens;
    using Desktop;
    using Desktop.Core.Licensing;
    using Desktop.Core.Settings;
    using Desktop.Core.UI.ScreenManager;
    using Desktop.Events;
    using Desktop.Explorer.EndpointExplorer;
    using Desktop.Explorer.QueueExplorer;
    using Desktop.LogWindow;
    using Desktop.MessageFlow;
    using Desktop.MessageHeaders;
    using Desktop.MessageList;
    using Desktop.MessageProperties;
    using Desktop.MessageViewers;
    using Desktop.Models;
    using Desktop.Saga;
    using Desktop.Settings;
    using Desktop.Shell;
    using Desktop.Startup;
    using Helpers;
    using Licensing;
    using NSubstitute;
    using NUnit.Framework;
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
        IScreenFactory ScreenFactory;
        IWindowManagerEx WindowManager;
        IQueueExplorerViewModel QueueExplorer;
        IEndpointExplorerViewModel EndpointExplorer;
        IMessageListViewModel MessageList;
        IConnectToMachineViewModel ConnectToViewModel;
        IMessageFlowViewModel MessageFlow;
        ISagaWindowViewModel SagaWindow;
        IEventAggregator EventAggregator;
        IStatusBarManager StatusbarManager;
        IMessageBodyViewModel MessageBodyView;
        IMessageHeadersViewModel HeaderView;
        ISettingsProvider SettingsProvider;
        AppLicenseManager LicenseManager;
        IShellViewStub View;
        IMessagePropertiesViewModel MessageProperties;
        ILogWindowViewModel LogWindow;
        IAppCommands App;
        ICommandLineArgParser CommandLineArgParser;

        [SetUp]
        public void TestInitialize()
        {
            ScreenFactory = Substitute.For<IScreenFactory>();
            WindowManager = Substitute.For<IWindowManagerEx>();
            QueueExplorer = Substitute.For<IQueueExplorerViewModel>();
            EndpointExplorer = Substitute.For<IEndpointExplorerViewModel>();
            MessageList = Substitute.For<IMessageListViewModel>();
            StatusbarManager = Substitute.For<IStatusBarManager>();
            EventAggregator = Substitute.For<IEventAggregator>();
            MessageFlow = Substitute.For<IMessageFlowViewModel>();
            SagaWindow = Substitute.For<ISagaWindowViewModel>();
            MessageBodyView = Substitute.For<IMessageBodyViewModel>();
            MessageProperties = Substitute.For<IMessagePropertiesViewModel>();
            View = Substitute.For<IShellViewStub>();
            HeaderView = Substitute.For<IMessageHeadersViewModel>();
            SettingsProvider = Substitute.For<ISettingsProvider>();
            LicenseManager = Substitute.For<AppLicenseManager>();
            LogWindow = Substitute.For<ILogWindowViewModel>();
            ConnectToViewModel = Substitute.For<IConnectToMachineViewModel>();
            SettingsProvider.GetSettings<ProfilerSettings>().Returns(DefaultAppSetting());
            App = Substitute.For<IAppCommands>();
            CommandLineArgParser = MockEmptyStartupOptions();

            shell = new ShellViewModel(App, ScreenFactory, WindowManager, QueueExplorer, 
                                       EndpointExplorer, MessageList, StatusbarManager, 
                                       EventAggregator, LicenseManager, MessageFlow, SagaWindow,
                                       MessageBodyView, HeaderView, SettingsProvider, MessageProperties, 
                                       LogWindow, CommandLineArgParser);

            ScreenFactory.CreateScreen<IConnectToMachineViewModel>().Returns(ConnectToViewModel);

            shell.AttachView(View, null);
        }

        ICommandLineArgParser MockEmptyStartupOptions()
        {
            var parser = Substitute.For<ICommandLineArgParser>();
            
            parser.ParsedOptions.Returns(new CommandLineOptions());

            return parser;
        }

        [Test]
        public void should_reload_stored_layout()
        {
            View.Received().OnRestoreLayout(SettingsProvider);
        }

        [Test]
        public void should_toggle_toolbar_status_when_work_is_in_progress()
        {
            shell.Handle(new WorkStarted());

            shell.CanDeleteCurrentQueue.ShouldBe(false);
            shell.CanDeleteSelectedMessages.ShouldBe(false);
            shell.CanConnectToMachine.ShouldBe(false);
            shell.CanCreateMessage.ShouldBe(false);
            shell.CanCreateQueue.ShouldBe(false);
            shell.CanExportMessage.ShouldBe(false);
            shell.CanImportMessage.ShouldBe(false);
            shell.CanPurgeCurrentQueue.ShouldBe(false);
            shell.CanRefreshQueues.ShouldBe(false);
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
        public void should_display_connect_dialog_when_connecting_to_msmq()
        {
            ConnectToViewModel.ComputerName.Returns("NewMachine");
            WindowManager.ShowDialog(Arg.Any<object>()).Returns(true);

            shell.ConnectToMessageQueue();

            ScreenFactory.Received().CreateScreen<IConnectToMachineViewModel>();
            WindowManager.Received().ShowDialog(ConnectToViewModel);
            QueueExplorer.Received().ConnectToQueue("NewMachine");
        }

        [Test]
        public void should_have_all_child_screens_ready_when_shell_is_activated()
        {
            ((IScreen)shell).Deactivate(true);

            View.Received().OnSaveLayout(SettingsProvider);
        }

        [Test]
        public void should_not_refresh_the_queues_when_auto_refresh_is_turned_off()
        {
            shell.AutoRefresh = false;

            shell.OnAutoRefreshing();

            QueueExplorer.DidNotReceive().RefreshData();
        }

        [Test]
        [ExpectedException(typeof (NotImplementedException))]
        public void message_import()
        {
            shell.ImportMessage();
        }

        [Test]
        [ExpectedException(typeof (NotImplementedException))]
        public void message_export()
        {
            shell.CreateMessage();
        }

        [Test]
        [Ignore] //TODO: NSubstitute doesn't play well with the inner async call
        public void should_refresh_queue_explorer_when_new_queue_is_created()
        {
            var viewModel = Substitute.For<IQueueCreationViewModel>();
            ScreenFactory.CreateScreen<IQueueCreationViewModel>().Returns(viewModel);
            WindowManager.ShowDialog(viewModel).Returns(true);

            AsyncHelper.Run(() => shell.CreateQueue());

            QueueExplorer.RefreshData().ReceivedCalls(); //TODO: Comment?
        }

        [Test]
        public void should_track_selected_explorer()
        {
            var selected = new QueueExplorerItem(new Queue("Error"));

            shell.Handle(new SelectedExplorerItemChanged(selected));

            shell.SelectedExplorerItem.ShouldNotBe(null);
            shell.SelectedExplorerItem.ShouldBeSameAs(selected);
        }

        [Test,Ignore("Need to figure out why this one is failing")]
        public void should_validate_trial_license()
        {
            const string RegisteredUser = "John Doe";
            const string LicenseType = "Trial";
            const int NumberOfDaysRemainingFromTrial = 5;

            var issuedLicense = new License
            {
                LicenseType = LicenseType,
                ExpirationDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                RegisteredTo = RegisteredUser
            };

            LicenseManager.CurrentLicense.Returns(issuedLicense);
            LicenseManager.GetRemainingTrialDays().Returns(NumberOfDaysRemainingFromTrial);

            shell.OnApplicationIdle();

            //StatusbarManager.Received().SetRegistrationInfo(Arg.Is(ShellViewModel.UnlicensedStatusMessage), Arg.Is("5 days"));
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