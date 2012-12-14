using System;
using System.Collections.Generic;
using Machine.Specifications;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core;
using NSubstitute;
using System.Linq;

namespace NServiceBus.Profiler.Tests.Queues
{
    [Subject("queue manager")]
    public abstract class with_a_queue_manager
    {
        protected static QueueManager QueueManager;
        protected static IQueueOperations QueueOperations;
        protected static IList<Queue> QueueList;
        protected static IList<MessageInfo> Messages;

        Establish context = () =>
        {
            QueueList = new List<Queue>(new[] { new Queue("localhost", "queue"), new Queue("localhost", "test") });
            Messages = new List<MessageInfo>(new[] { new MessageInfo(), new MessageInfo() });

            QueueOperations = Substitute.For<IQueueOperations>();
            QueueOperations.GetQueues(Arg.Any<string>()).Returns(QueueList);
            QueueOperations.GetMessages(Arg.Any<Queue>()).Returns(Messages);

            QueueManager = new QueueManager(QueueOperations);
        };
    }

    [Subject("queue manager")]
    public class when_manager_is_initialized : with_a_queue_manager
    {
        protected static IList<Queue> FetchedQueues;

        Because of = () => FetchedQueues = QueueManager.GetQueues();

        It should_have_loaded_queue_information = () => FetchedQueues.ShouldNotBeNull();

        It should_have_correct_amount_of_queues = () => FetchedQueues.Count.ShouldEqual(2);
    }

    [Subject("queue manager")]
    public class when_creating_queues : with_a_queue_manager
    {
        protected static Queue OperatingQueue;

        Because of = () =>
        {
            var address = new Address("NewMachine", "NewQueue");
            var queue = new Queue(address);
            QueueManager.CreatePrivateQueue(queue);
        };

        It should_create_private_queue_on_the_queue_subsystem = () => QueueOperations.Received().CreateQueue(Arg.Is<Queue>(x => x.Address.Equals("newqueue@newmachine")), Arg.Is(true));
    }

    [Subject("queue manager")]
    public class when_purging_the_queue : with_a_queue_manager
    {
        protected static IList<Queue> FetchedQueues;
        protected static Queue OperatingQueue;
            
        Because of = () =>
        {
            FetchedQueues = QueueManager.GetQueues();
            OperatingQueue = FetchedQueues.First();
            QueueManager.Purge(OperatingQueue);
        };

        It should_remove_all_messages_from_the_queue_subsystem = () => QueueOperations.Received().PurgeQueue(Arg.Is(OperatingQueue));
    }

    [Subject("queue manager")]
    public class when_fetching_messages : with_a_queue_manager
    {
        Because of = () => QueueManager.GetMessages(QueueList.First());

        It should_fetch_messages_from_the_queue_subsystem = () => QueueOperations.Received().GetMessages(Arg.Any<Queue>());
    }

    [Subject("queue manager")]
    public class when_deleting_messages : with_a_queue_manager
    {
        protected static MessageInfo MessageToDelete;
        protected static Queue OperatingQueue;

        Because of = () =>
        {
            OperatingQueue = QueueList.First();
            MessageToDelete = QueueManager.GetMessages(OperatingQueue).First();

            QueueManager.DeleteMessage(OperatingQueue, MessageToDelete);
        };

        It should_remove_the_message_from_queue_subsystem = () => QueueOperations.Received().DeleteMessage(OperatingQueue, MessageToDelete.Id);
    }
}