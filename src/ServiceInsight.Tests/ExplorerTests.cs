namespace Particular.ServiceInsight.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Screens;
    using Desktop.Core;
    using Desktop.Core.UI.ScreenManager;
    using Desktop.Events;
    using Desktop.Explorer;
    using Desktop.Explorer.QueueExplorer;
    using Desktop.Models;
    using Helpers;
    using NSubstitute;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class ExplorerTests
    {
        private QueueExplorerViewModel explorer;
        private IExplorerView view;
        private IQueueManagerAsync queueManager;
        private IEventAggregator eventAggregator;
        private IWindowManagerEx windowManagerEx;
        private INetworkOperations networkOperations;
        private Queue queue;
        private Queue subQueue;
        private QueueExplorerItem queueNode;

        [SetUp]
        public void TestInitialize()
        {
            queueManager = Substitute.For<IQueueManagerAsync>();
            view = Substitute.For<IExplorerView>();
            eventAggregator = Substitute.For<IEventAggregator>();
            windowManagerEx = Substitute.For<IWindowManagerEx>();
            networkOperations = Substitute.For<INetworkOperations>();
            explorer = new QueueExplorerViewModel(queueManager, eventAggregator, windowManagerEx, networkOperations);

            queue = new Queue("TestQueue");
            subQueue = new Queue("TestQueue.Subscriptions");

            IList<Queue> queues = new List<Queue> { queue, subQueue };
            queueManager.GetQueues(Arg.Any<string>()).Returns(Task.Run(() => queues));
            queueManager.GetQueues().Returns(Task.Run(() => queues));
            queueManager.GetMessageCount(Arg.Any<Queue>()).Returns(Task.Run(() => queues.Count));
            queueManager.IsMsmqInstalled(Arg.Any<string>()).Returns(Task.Run(() => true));

            AsyncHelper.Run(() => explorer.AttachView(view, null));
            AsyncHelper.Run(() => explorer.ConnectToQueue(Environment.MachineName));

            queueNode = explorer.MachineRoot.Children.OfType<QueueExplorerItem>().First();
        }

        [Test]
        public void should_publish_the_message_that_queue_is_selected()
        {
            explorer.SelectedNode = queueNode;
            eventAggregator.Received(1).Publish(Arg.Any<SelectedExplorerItemChanged>());
        }

        [Test]
        public void should_have_a_selected_queue()
        {
            explorer.SelectedNode = queueNode;
            explorer.SelectedQueue.ShouldNotBe(null);
        }

        [Test]
        public void should_have_the_same_queue_selected()
        {
            explorer.SelectedNode = queueNode;
            explorer.SelectedQueue.ShouldBeSameAs(queue);
        }

        [Test]
        public void should_ask_for_user_confirmation_before_deleting_the_queue()
        {
            windowManagerEx.ShowMessageBox(Arg.Any<string>()).ReturnsForAnyArgs(MessageBoxResult.OK);

            explorer.SelectedNode = queueNode;
            explorer.DeleteSelectedQueue();

            windowManagerEx.ReceivedWithAnyArgs(1).ShowMessageBox(Arg.Any<string>());
            queueManager.Received(1).DeleteQueue(Arg.Is(queue));
        }

        [Test]
        public void should_not_delete_the_message_when_cancelling_queue_deletion()
        {
            windowManagerEx.ShowMessageBox(Arg.Any<string>()).ReturnsForAnyArgs(MessageBoxResult.Cancel);

            explorer.DeleteSelectedQueue();

            queueManager.DidNotReceive().DeleteQueue(Arg.Any<Queue>());
        }

        [Test]
        public void should_ask_view_to_expand_tree_nodes_when_expanding_tree_nodes()
        {
            explorer.ExpandNodes();

            view.Received(1).Expand();
        }

        [Test]
        public void should_display_connected_server_when_loading_queue_from_the_connected_server()
        {
            var anotherQueue = new Queue("SecondQueue");
            IList<Queue> queues = new List<Queue> { queue, anotherQueue };
            queueManager.GetQueues(Arg.Any<string>()).Returns(Task.FromResult(queues));

            AsyncHelper.Run(() => explorer.ConnectToQueue(Environment.MachineName));
            AsyncHelper.Run(() => explorer.RefreshData());

            explorer.Items[0].ShouldBeTypeOf<QueueServerExplorerItem>();
            explorer.Items.Count.ShouldBe(1);
            explorer.MachineRoot.Children.Count.ShouldBe(2);
        }

        [Test]
        public void should_not_delete_the_queue_when_user_do_not_confirm_queue_deletion()
        {
            windowManagerEx.ShowMessageBox(Arg.Any<string>()).ReturnsForAnyArgs(MessageBoxResult.No);

            explorer.DeleteSelectedQueue();

            queueManager.DidNotReceiveWithAnyArgs().DeleteQueue(Arg.Any<Queue>());
        }

        [Test]
        public void should_automatically_connect_to_local_machine_when_explorer_is_activated()
        {
            var q = new Queue("FirstQueue");
            IList<Queue> queues = new List<Queue> { q };
            queueManager.GetQueues(Arg.Any<string>()).Returns(Task.FromResult(queues));

            ((IScreen)explorer).Activate();

            explorer.ConnectedToAddress.ShouldBe(Environment.MachineName.ToLower());
        }

        [Test]
        public void should_refresh_message_count_in_the_tree_node_when_messages_are_loaded()
        {
            var q = new Queue("FirstQueue");
            IList<Queue> queues = new List<Queue> { q };
            queueManager.GetQueues(Arg.Any<string>()).Returns(Task.FromResult(queues));

            AsyncHelper.Run(() => explorer.ConnectToQueue(Environment.MachineName));
            
            explorer.SelectedNode = new QueueExplorerItem(q);
            explorer.Handle(new QueueMessageCountChanged(q, 5));

            explorer.SelectedNode.DisplayName.ShouldContain("(5)");
        }

        [Test]
        public void should_throw_on_non_existing_machine_names()
        {
            Should.Throw<Exception>(() => AsyncHelper.Run(() => explorer.ConnectToQueue("NonExistingMachine")));
        }

        [Test]
        public void should_not_throw_an_exception_when_system_queues_are_orphaned()
        {
            IList<Queue> unorderedQueueList = new List<Queue>(new[] { new Queue("myqueue.subscriptions") });
            queueManager = Substitute.For<IQueueManagerAsync>();
            queueManager.GetQueues().ReturnsForAnyArgs(Task.FromResult(unorderedQueueList));
            explorer = new QueueExplorerViewModel(queueManager, eventAggregator, windowManagerEx, networkOperations);

            Should.NotThrow(() => explorer.RefreshData());
        }
    }
}