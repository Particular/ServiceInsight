using System;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using ExceptionHandler;
using NServiceBus.Profiler.Desktop;
using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.Core.Licensing;
using NServiceBus.Profiler.Desktop.Core.Settings;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer;
using NServiceBus.Profiler.Desktop.Explorer.QueueExplorer;
using NServiceBus.Profiler.Desktop.LogWindow;
using NServiceBus.Profiler.Desktop.MessageFlow;
using NServiceBus.Profiler.Desktop.MessageList;
using NServiceBus.Profiler.Desktop.MessageProperties;
using NServiceBus.Profiler.Desktop.MessageViewers;
using NServiceBus.Profiler.Desktop.Models;
using NServiceBus.Profiler.Desktop.ScreenManager;
using NServiceBus.Profiler.Desktop.Settings;
using NServiceBus.Profiler.Desktop.Shell;
using NServiceBus.Profiler.Tests.Helpers;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace NServiceBus.Profiler.Tests
{
    public interface IShellViewStub : IShellView
    {
        bool IsOpen { get; set; }
        void Close();
    }

    [TestFixture]
    public class ShellViewModelTests
    {
        private ShellViewModel shell;
        private IScreenFactory ScreenFactory;
        private IWindowManagerEx WindowManager;
        private IQueueExplorerViewModel QueueExplorer;
        private IEndpointExplorerViewModel EndpointExplorer;
        private IMessageListViewModel MessageList;
        private ConnectToMachineViewModel ConnectToViewModel;
        private INetworkOperations NetworkOperations;
        private IMessageFlowViewModel MessageFlow;
        private IEventAggregator EventAggregator;
        private IExceptionHandler ExceptionHandler;
        private IStatusBarManager StatusbarManager;
        private IMessageBodyViewModel MessageBodyView;
        private ISettingsProvider SettingsProvider;
        private ILicenseManager LicenseManager;
        private IShellViewStub View;
        private IMessagePropertiesViewModel MessageProperties;
        private ILogWindowViewModel LogWindow;
        private IAppCommands App;

        [SetUp]
        public void TestInitialize()
        {
            ScreenFactory = Substitute.For<IScreenFactory>();
            WindowManager = Substitute.For<IWindowManagerEx>();
            QueueExplorer = Substitute.For<IQueueExplorerViewModel>();
            EndpointExplorer = Substitute.For<IEndpointExplorerViewModel>();
            MessageList = Substitute.For<IMessageListViewModel>();
            NetworkOperations = Substitute.For<INetworkOperations>();
            ExceptionHandler = Substitute.For<IExceptionHandler>();
            StatusbarManager = Substitute.For<StatusBarManager>();
            EventAggregator = Substitute.For<IEventAggregator>();
            MessageFlow = Substitute.For<IMessageFlowViewModel>();
            MessageBodyView = Substitute.For<IMessageBodyViewModel>();
            MessageProperties = Substitute.For<IMessagePropertiesViewModel>();
            View = Substitute.For<IShellViewStub>();
            SettingsProvider = Substitute.For<ISettingsProvider>();
            LicenseManager = Substitute.For<ILicenseManager>();
            LogWindow = Substitute.For<ILogWindowViewModel>();
            ConnectToViewModel = Substitute.For<ConnectToMachineViewModel>(NetworkOperations);
            SettingsProvider.GetSettings<ProfilerSettings>().Returns(DefaultAppSetting());
            App = Substitute.For<IAppCommands>();
            shell = new ShellViewModel(App, ScreenFactory, WindowManager, QueueExplorer, EndpointExplorer, MessageList,
                                       StatusbarManager, EventAggregator, LicenseManager, MessageFlow, MessageBodyView,
                                       SettingsProvider, MessageProperties, LogWindow);

            ScreenFactory.CreateScreen<ConnectToMachineViewModel>().Returns(ConnectToViewModel);

            shell.AttachView(View, null);
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

            shell.ConnectToMachine();

            ScreenFactory.Received().CreateScreen<ConnectToMachineViewModel>();
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

            QueueExplorer.DidNotReceive().PartialRefresh();
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
        public void should_refresh_queue_exporer_when_new_queue_is_created()
        {
            var viewModel = Substitute.For<IQueueCreationViewModel>();
            ScreenFactory.CreateScreen<IQueueCreationViewModel>().Returns(viewModel);
            WindowManager.ShowDialog(viewModel).Returns(true);

            AsyncHelper.Run(() => shell.CreateQueue());

            QueueExplorer.FullRefresh().ReceivedCalls(); //TODO: Comment?
        }

        [Test]
        public void should_track_selected_explorer()
        {
            var selected = new QueueExplorerItem(new Queue("Error"));

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

            var IssuedLicense = new ProfilerLicense
            {
                LicenseType = LicenseType,
                ExpirationDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                RegisteredTo = RegisteredUser
            };

            LicenseManager.CurrentLicense.Returns(IssuedLicense);
            LicenseManager.GetRemainingTrialDays().Returns(NumberOfDaysRemainingFromTrial);

            shell.OnApplicationIdle();

            StatusbarManager.Registration.ShouldContain(NumberOfDaysRemainingFromTrial.ToString());
            StatusbarManager.Registration.ShouldContain("Unlicensed");
        }

        [TearDown]
        public void TestCleanup()
        {
            ((IScreen)shell).Deactivate(true);
        }

        private static ProfilerSettings DefaultAppSetting()
        {
            return new ProfilerSettings
            {
                AutoRefreshTimer = 15,
            };
        }

    }
}