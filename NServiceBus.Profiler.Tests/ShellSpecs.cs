using System;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using ExceptionHandler;
using Machine.Specifications;
using NServiceBus.Profiler.Desktop;
using NServiceBus.Profiler.Desktop.Conversations;
using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.Core.Licensing;
using NServiceBus.Profiler.Desktop.Core.Settings;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Explorer;
using NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer;
using NServiceBus.Profiler.Desktop.Explorer.QueueExplorer;
using NServiceBus.Profiler.Desktop.ExtensionMethods;
using NServiceBus.Profiler.Desktop.LogWindow;
using NServiceBus.Profiler.Desktop.MessageHeaders;
using NServiceBus.Profiler.Desktop.MessageList;
using NServiceBus.Profiler.Desktop.MessageViewers;
using NServiceBus.Profiler.Desktop.Models;
using NServiceBus.Profiler.Desktop.ScreenManager;
using NServiceBus.Profiler.Desktop.Shell;
using NSubstitute;

namespace NServiceBus.Profiler.Tests.Shell
{
    public interface IShellViewStub : IShellView
    {
        bool IsOpen { get; set; }
        void Close();
    }

    [Subject("shell")]
    public class with_a_shell
    {
        protected static ShellViewModel shell;
        protected static IScreenFactory ScreenFactory;
        protected static IWindowManagerEx WindowManager;
        protected static IQueueExplorerViewModel QueueExplorer;
        protected static IEndpointExplorerViewModel EndpointExplorer;
        protected static IMessageListViewModel MessageList;
        protected static ConnectToMachineViewModel ConnectToViewModel;
        protected static INetworkOperations NetworkOperations;
        protected static IConversationViewModel Conversation;
        protected static IEventAggregator EventAggregator;
        protected static IExceptionHandler ExceptionHandler;
        protected static IStatusBarManager StatusbarManager;
        protected static IMessageBodyViewModel MessageBodyView;
        protected static ISettingsProvider SettingsProvider;
        protected static ILicenseManager LicenseManager;
        protected static IShellViewStub View;
        protected static IMessagePropertiesViewModel MessageProperties;
        protected static ILogWindowViewModel LogWindow;
        protected static IAppCommands App;

        Establish context = () =>
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
            Conversation = Substitute.For<IConversationViewModel>();
            MessageBodyView = Substitute.For<IMessageBodyViewModel>();
            MessageProperties = Substitute.For<IMessagePropertiesViewModel>();
            View = Substitute.For<IShellViewStub>();
            SettingsProvider = Substitute.For<ISettingsProvider>();
            LicenseManager = Substitute.For<ILicenseManager>();
            LogWindow = Substitute.For<ILogWindowViewModel>();
            ConnectToViewModel = Substitute.For<ConnectToMachineViewModel>(NetworkOperations);
            App = Substitute.For<IAppCommands>();
            shell = new ShellViewModel(App, ScreenFactory, WindowManager, QueueExplorer, EndpointExplorer, MessageList,
                                       StatusbarManager, EventAggregator, LicenseManager, Conversation, MessageBodyView,
                                       SettingsProvider, MessageProperties, LogWindow);

            ScreenFactory.CreateScreen<ConnectToMachineViewModel>().Returns(ConnectToViewModel);

            shell.AttachView(View, null);
        };

        It should_reload_stored_layout = () => View.Received().RestoreLayout(SettingsProvider);

        Cleanup after = () => ((IScreen)shell).Deactivate(true);
    }

    public class when_there_is_work_in_progress : with_a_shell
    {
        Because of = () => shell.Handle(new WorkStarted());

        It should_not_allow_deleting_the_queue = () => shell.CanDeleteCurrentQueue.ShouldBeFalse();
        It should_not_allow_deleting_the_messages = () => shell.CanDeleteSelectedMessages.ShouldBeFalse();
        It should_not_allow_connecting_to_another_machine = () => shell.CanConnectToMachine.ShouldBeFalse();
        It should_not_allow_creating_new_messages = () => shell.CanCreateMessage.ShouldBeFalse();
        It should_not_allow_creating_new_queue = () => shell.CanCreateQueue.ShouldBeFalse();
        It should_not_allow_exporting_messages = () => shell.CanExportMessage.ShouldBeFalse();
        It should_not_allow_importing_messages = () => shell.CanImportMessage.ShouldBeFalse();
        It should_not_allow_purging_the_queue = () => shell.CanPurgeCurrentQueue.ShouldBeFalse();
        It should_not_allow_refreshing_the_queue = () => shell.CanRefreshQueues.ShouldBeFalse();
    }

    public class when_the_work_is_partially_finished : with_a_shell
    {
        Establish context = () =>
        {
            shell.Handle(new WorkStarted("Some Work"));
            shell.Handle(new WorkStarted("Some Other Work"));
        };

        Because of = () => shell.Handle(new WorkFinished());

        It should_still_report_another_work_in_progress = () => shell.WorkInProgress.ShouldBeTrue();
    }

    public class when_the_work_is_finished : with_a_shell
    {
        Establish context = () => shell.Handle(new WorkStarted());

        Because of = () => shell.Handle(new WorkFinished());

        It should_finish_all_the_works_in_progress = () => shell.WorkInProgress.ShouldBeFalse();
    }

    public class when_more_work_is_finished_than_started : with_a_shell
    {
        Establish context = () => shell.Handle(new WorkStarted());

        Because of = () =>
        {
            shell.Handle(new WorkFinished());
            shell.Handle(new WorkFinished());
        };

        It should_have_finished_the_work_in_progress = () => shell.WorkInProgress.ShouldBeFalse();
    }

    public class when_connecting_to_msmq : with_a_shell
    {
        Because of = () =>
        {
            ConnectToViewModel.ComputerName.Returns("NewMachine");
            WindowManager.ShowDialog(Arg.Any<object>(), Arg.Any<object>()).Returns(true);

            shell.ConnectToMachine();
        };

        It should_display_connect_dialog = () =>
        {
            ScreenFactory.Received().CreateScreen<ConnectToMachineViewModel>();
            WindowManager.Received().ShowDialog(ConnectToViewModel);
        };

        It should_connect_explorer_to_the_msmq_machine = () => QueueExplorer.Received().ConnectToQueue("NewMachine");

    }

    public class when_shell_activated : with_a_shell
    {
        Because of = () => ((IScreen)shell).Activate();

        It should_have_all_child_screens_ready = () => shell.Items.ForEach(x => x.Received().Activate());
    }

    public class when_shell_deactivated : with_a_shell
    {
        Because of = () => ((IScreen)shell).Deactivate(true);

        It should_persist_layout_in_the_setting_storage = () => View.Received().SaveLayout(SettingsProvider);
    }

    public class when_turning_off_auto_refresh : with_a_shell
    {
        Establish context = () => shell.AutoRefresh = false;

        Because of = () => shell.OnAutoRefreshing();

        It should_not_refresh_the_queues = () => QueueExplorer.DidNotReceive().PartialRefresh();
    }

    public class when_importing_messages : with_a_shell
    {
        It is_still_not_implemented = () => Catch.Exception(() => shell.ImportMessage());
    }

    public class when_exporting_messages : with_a_shell
    {
        It is_still_not_implemented = () => Catch.Exception(() => shell.ExportMessage());
    }

    public class when_creating_new_messages : with_a_shell
    {
        It is_still_not_implemented = () => Catch.Exception(() => shell.CreateMessage());
    }

    public class when_creating_new_queue : with_a_shell
    {
        protected static IQueueCreationViewModel ViewModel;

        Establish context = () =>
        {
            ViewModel = Substitute.For<IQueueCreationViewModel>();
            
            ScreenFactory.CreateScreen<IQueueCreationViewModel>().Returns(ViewModel);
            WindowManager.ShowDialog(ViewModel, null).Returns(true);
        };

        Because of = () => shell.CreateQueue();

        //TODO: Upgrade to latest NSubstitute and use CancelAsync?
        //It should_refresh_exporer = () => shell.QueueExplorer.FullRefresh().ReceivedCalls();
    }

    public class when_selecting_an_item_in_the_explorer : with_a_shell
    {
        protected static ExplorerItem Selected;

        Establish context = () => Selected = new QueueExplorerItem(new Queue("Error"));

        Because of = () => shell.Handle(new SelectedExplorerItemChanged(Selected));

        It should_have_a_selection = () => shell.SelectedExplorerItem.ShouldNotBeNull();
        It should_be_the_same_selection = () => shell.SelectedExplorerItem.ShouldBeTheSameAs(Selected);
    }

    public class when_validation_application_with_trial_license : with_a_shell
    {
        protected static ProfilerLicense IssuedLicense;
        protected static string RegisteredUser = "John Doe";
        protected static string LicenseType = "Trial";
        protected static int NumberOfDaysRemainingFromTrial = 5;

        Establish context = () =>
        {
            IssuedLicense = new ProfilerLicense
            {
                LicenseType = LicenseType,
                ExpirationDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                RegisteredTo = RegisteredUser
            };

            LicenseManager.CurrentLicense.Returns(IssuedLicense);
            LicenseManager.GetRemainingTrialDays().Returns(NumberOfDaysRemainingFromTrial);
        };

        Because of = () => shell.OnApplicationIdle();

        It should_display_license_owner_on_statusbar = () => StatusbarManager.Registration.ShouldContain(NumberOfDaysRemainingFromTrial.ToString());
        It should_display_license_type_on_statusbar = () => StatusbarManager.Registration.ShouldContain("Unlicensed");
    }
}