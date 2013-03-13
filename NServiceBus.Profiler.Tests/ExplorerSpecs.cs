using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.PresentationFramework.ApplicationModel;
using Machine.Specifications;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core;
using NServiceBus.Profiler.Desktop.Explorer;
using NServiceBus.Profiler.Desktop.Explorer.QueueExplorer;
using NServiceBus.Profiler.Desktop.ScreenManager;
using NSubstitute;
using System.Linq;

namespace NServiceBus.Profiler.Tests.Explorer
{
    [Subject("queueExplorer")]
    public abstract class with_the_explorer
    {
        protected static IQueueExplorerViewModel Explorer;
        protected static IExplorerView View;
        protected static IQueueManager QueueManager;
        protected static IEventAggregator EventAggregator;
        protected static IWindowManagerEx WindowManagerEx;
        protected static Queue Queue;
        protected static Queue SubQueue;
        protected static QueueExplorerItem QueueNode;

        Establish context = () =>
        {
            QueueManager = Substitute.For<IQueueManager>();
            View = Substitute.For<IExplorerView>();
            EventAggregator = Substitute.For<IEventAggregator>();
            WindowManagerEx = Substitute.For<IWindowManagerEx>();

            Explorer = new QueueExplorerViewModel(QueueManager, EventAggregator, WindowManagerEx);

            Queue = new Queue("TestQueue");
            SubQueue = new Queue("TestQueue.Subscriptions");

            QueueManager.GetQueues().ReturnsForAnyArgs(new[] { Queue, SubQueue });

            Explorer.AttachView(View, null);
            QueueNode = Explorer.MachineRoot.Children.OfType<QueueExplorerItem>().First();
        };
    }

    public class when_a_queue_selected : with_the_explorer
    {
        Establish context = () => Explorer.SelectedNode = QueueNode;

        It should_publish_the_message_that_queue_is_selected = () => EventAggregator.Received(1).Publish(Arg.Any<SelectedQueueChanged>());
        It should_have_a_selected_queue = () => Explorer.SelectedQueue.ShouldNotBeNull();
        It should_have_the_same_queue_selected = () => Explorer.SelectedQueue.ShouldBeTheSameAs(Queue);
    }

    public class when_deleting_a_selected_queue : when_a_queue_selected
    {
        Establish context = () => WindowManagerEx.ShowMessageBox(Arg.Any<string>()).ReturnsForAnyArgs(MessageBoxResult.OK);

        Because of = () => Explorer.DeleteSelectedQueue();

        It should_ask_for_user_confirmation_before_deleting_the_queue = () => WindowManagerEx.ReceivedWithAnyArgs(1).ShowMessageBox(Arg.Any<string>());
        It should_delete_the_message_from_the_queue = () => QueueManager.Received(1).DeleteQueue(Arg.Is(Queue));
    }

    public class when_cancelling_queue_deletion : when_a_queue_selected
    {
        Establish context = () => WindowManagerEx.ShowMessageBox(Arg.Any<string>()).ReturnsForAnyArgs(MessageBoxResult.Cancel);

        Because of = () => Explorer.DeleteSelectedQueue();

        It should_not_delete_the_message = () => QueueManager.DidNotReceive().DeleteQueue(Arg.Any<Queue>());
    }

    public class when_expanding_tree_nodes : with_the_explorer
    {
        Because of = () => Explorer.ExpandNodes();

        It should_ask_view_to_expand_tree_nodes = () => View.Received(1).Expand();
    }

    public class when_loading_queue_from_the_connected_server : with_the_explorer
    {
        protected static Queue AnotherQueue;
        protected static IList<Queue> Queues;

        Establish context = () =>
        {
            AnotherQueue = new Queue("SecondQueue");
            Queues = new[] { Queue, AnotherQueue };
            QueueManager.GetQueues(Arg.Any<string>()).Returns(Queues);

            Explorer.ConnectToQueue(Environment.MachineName);
        };

        Because of = () => Explorer.PartialRefresh();

        It should_display_connected_server = () => Explorer.Items[0].ShouldBeOfType<ServerExplorerItem>();
        It should_have_only_server_node = () => Explorer.Items.Count.ShouldEqual(1);
        It should_have_queue_for_the_server_as_child_nodes = () => Explorer.MachineRoot.Children.Count.ShouldEqual(2);
    }

    public class when_user_do_not_confirm_queue_deletion : with_the_explorer
    {
        Establish context = () => WindowManagerEx.ShowMessageBox(Arg.Any<string>()).ReturnsForAnyArgs(MessageBoxResult.No);

        Because of = () => Explorer.DeleteSelectedQueue();

        It should_not_delete_the_queue = () => QueueManager.DidNotReceiveWithAnyArgs().DeleteQueue(Arg.Any<Queue>());
    }

    public class when_explorer_is_activated : with_the_explorer
    {
        protected static IList<Queue> Queues;

        Establish context = () =>
        {
            Queue = new Queue("FirstQueue");
            Queues = new[] {Queue};
            QueueManager.GetQueues(Arg.Any<string>()).Returns(Queues);
        };

        Because of = () => Explorer.Activate();

        It should_automatically_connect_to_local_machine = () => Explorer.ConnectedToAddress.ShouldEqual(Environment.MachineName.ToLower());
    }

    public class when_messages_are_loaded : with_the_explorer
    {
        protected static IList<Queue> Queues;

        Establish context = () =>
        {
            Queue = new Queue("FirstQueue");
            Queues = new[] {Queue};
            QueueManager.GetQueues(Arg.Any<string>()).Returns(Queues);

            Explorer.ConnectToQueue(Environment.MachineName);
            Explorer.SelectedNode = new QueueExplorerItem(Queue);
        };

        Because of = () => Explorer.Handle(new QueueMessageCountChanged(Queue, 5));

        It should_refresh_message_count_in_the_tree_node = () => Explorer.SelectedNode.DisplayName.ShouldContain("(5)"); //TODO: Fix broken test
    }

    public class when_auto_refresh_event_is_triggerred : with_the_explorer
    {
        protected static Queue Q;

        Establish context = () => QueueManager.GetMessageCount(Arg.Any<Queue>()).Returns(5);

        Because of = () =>
        {
            QueueManager.ClearReceivedCalls();
            Explorer.Handle(new AutoRefreshBeat());
        };

        It should_get_new_message_count = () => QueueManager.Received(1).GetMessageCount(Arg.Any<Queue>()); 
        It should_refresh_message_count_for_the_queue = () => Explorer.MachineRoot.Children.First(x => x.DisplayName.Contains("(5)"));
    }

    public class when_auto_refresh_event_is_triggered_without_being_connected_to_any_machine : with_the_explorer
    {
        protected static Queue Q;

        Establish context = () => Explorer.Items.Clear();

        Because of = () =>
        {
            QueueManager.ClearReceivedCalls();
            Explorer.Handle(new AutoRefreshBeat());
        };

        It should_not_refresh_any_message_count = () => QueueManager.DidNotReceive().GetMessageCount(Arg.Any<Queue>());
    }

    public class when_nservicebus_system_queues_are_present_display_them_as_subqueues : with_the_explorer
    {
        Because of = () => Explorer.PartialRefresh();
    }

    public class when_connected_to_another_machine : with_the_explorer
    {
        protected static Exception Error;

        Because of = () => Error = Catch.Exception(() => Explorer.ConnectToQueue("NonExistingMachine"));

        It should_throw_on_non_existing_machine_names = () => Error.ShouldNotBeNull();
    }

    public class when_system_queues_are_orphaned : with_the_explorer
    {
        protected static List<Queue> UnorderedQueueList;
        protected static Exception Error;

        Establish context = () =>
        {
            UnorderedQueueList = new List<Queue>(new[] { new Queue("myqueue.subscriptions") });
            QueueManager = Substitute.For<IQueueManager>();
            QueueManager.GetQueues().ReturnsForAnyArgs(UnorderedQueueList);
            Explorer = new QueueExplorerViewModel(QueueManager, EventAggregator, WindowManagerEx);
        };

        Because of = () => Error = Catch.Exception(() => Explorer.FullRefresh());

        It should_not_throw_an_exception = () => Error.ShouldBeNull();
    }
}