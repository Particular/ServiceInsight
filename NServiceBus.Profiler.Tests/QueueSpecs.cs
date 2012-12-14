using System;
using System.Linq;
using Machine.Specifications;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core;

namespace NServiceBus.Profiler.Tests.Queues
{
    [Subject("queues")]
    public class when_local_queue_initialized
    {
        protected static string QueueName;
        protected static Queue Queue;
        protected static QueueManager Manager;

        Establish context = () =>
        {
            QueueName = Guid.NewGuid().ToString("N");
            Manager = new QueueManager();
            Manager.CreatePrivateQueue(new Queue(Environment.MachineName, QueueName), true);
        };

        Because of = () => { Queue = Manager.GetQueues().FirstOrDefault(q => q.Address.Queue == QueueName); };

        It should_have_mapped_the_queue_information = () => Queue.ShouldNotBeNull();

        It should_be_transactional = () => Queue.IsTransactional.ShouldBeTrue();

        It should_have_formatname_mapped = () => Queue.FormatName.ShouldNotBeEmpty();

        Cleanup after = () => Manager.DeleteQueue(Queue);
    }

    [Subject("queues")]
    public class when_connected_to_a_queue
    {
        protected static string QueueName;
        protected static Queue Queue;
        protected static QueueManager Manager;

        Establish context = () =>
        {
            QueueName = Guid.NewGuid().ToString("N");
            Manager = new QueueManager();
            Queue = Manager.CreatePrivateQueue(new Queue(new Address(Environment.MachineName, QueueName)));
        };

        Because of = () => { Queue = Manager.GetQueues().FirstOrDefault(q => q.Address.Queue == QueueName); };

        It should_say_the_queue_is_a_local_queue = () => Queue.IsRemoteQueue().ShouldBeFalse();
        It should_have_queue_name_when_converted_to_string = () => Queue.ToString().ShouldContain(QueueName);
        It should_have_machine_name_when_converted_to_string = () => Queue.ToString().ShouldContain(Environment.MachineName.ToLower());

        Cleanup after = () => Manager.DeleteQueue(Queue);
    }

    [Subject("queues")]
    public class when_moving_messages_across_queues : with_a_message_in_the_queue
    {
        protected static MessageInfo Message;

        Establish context = () => Message = QueueManager.GetMessages(SourceQ).First();

        Because of = () => QueueManager.MoveMessage(SourceQ, DestQ, Message.Id);

        It destination_should_have_the_message = () => QueueManager.GetMessages(DestQ).Count.ShouldEqual(1);
    }

    public class moving_non_existing_messages_across_queues : with_a_message_in_the_queue
    {
        protected static MessageInfo Message;
        protected static Exception Error;

        Establish context = () =>
        {
            Message = QueueManager.GetMessages(SourceQ).First();
            QueueManager.DeleteMessage(SourceQ, Message);
        };

        Because of = () => Error = Catch.Exception(() => QueueManager.MoveMessage(SourceQ, DestQ, Message.Id));

        It should_throw_an_exception = () => Error.ShouldNotBeNull();
        It should_throw_queue_manager_exception = () => Error.ShouldBeOfType<QueueManagerException>();
        It should_have_detail_in_queue_manager_exception = () => Error.Message.ShouldContain("Message could not be loaded");
    }

    public class moving_message_to_non_transactional_queue : with_a_message_in_the_queue
    {
        protected static MessageInfo Message;
        protected static Queue NonTransactionalQ;
        protected static Exception Error;

        Establish context = () =>
        {
            Message = QueueManager.GetMessages(SourceQ).First();
            NonTransactionalQ = QueueManager.CreatePrivateQueue(new Queue(new Address("Nontransactional")), transactional: false);
        };

        Because of = () => Error = Catch.Exception(() => QueueManager.MoveMessage(SourceQ, NonTransactionalQ, Message.Id));

        It should_throw_an_exception = () => Error.ShouldNotBeNull();
        It should_throw_queue_manager_exception = () => Error.ShouldBeOfType<QueueManagerException>();
        It should_have_detail_in_error_message = () => Error.Message.ShouldContain("is not transactional");

        Cleanup cleanup = () => QueueManager.DeleteQueue(NonTransactionalQ);
    }

    public class counting_messages_in_the_queue : with_a_message_in_the_queue
    {
        protected static int MessageCount;

        Because of = () => MessageCount = QueueManager.GetMessageCount(SourceQ);

        It should_report_message_count = () => MessageCount.ShouldEqual(2);
    }

    public class creating_an_existing_queue : with_a_message_in_the_queue
    {
        protected static Queue ReCreatedQueue;

        Because of = () => ReCreatedQueue = QueueManager.CreatePrivateQueue(SourceQ);

        It should_have_the_same_queue_name_as_source = () => ReCreatedQueue.FormatName.ShouldEqual(SourceQ.FormatName);
        It should_have_the_same_address_as_source = () => ReCreatedQueue.Address.ShouldEqual(SourceQ.Address);
        It should_have_the_same_transactional_flag_as_source = () => ReCreatedQueue.IsTransactional.ShouldEqual(SourceQ.IsTransactional);
        It should_have_the_same_read_permission_as_source = () => ReCreatedQueue.CanRead.ShouldEqual(SourceQ.CanRead);
    }

    [Subject("queue")]
    public abstract class with_a_message_in_the_queue
    {
        protected static Queue SourceQ;
        protected static Queue DestQ;
        protected static QueueManager QueueManager;

        Establish context = () =>
        {
            QueueManager = new QueueManager();
            SourceQ = QueueManager.CreatePrivateQueue(new Queue(new Address(Environment.MachineName, Guid.NewGuid().ToString("N"))));
            DestQ = QueueManager.CreatePrivateQueue(new Queue(new Address(Environment.MachineName, Guid.NewGuid().ToString("N"))));
            QueueManager.SendMessage(SourceQ, "This is a test message");
            QueueManager.SendMessage(SourceQ, "This the second test message");
        };

        Cleanup cleanup = () =>
        {
            QueueManager.DeleteQueue(SourceQ);
            QueueManager.DeleteQueue(DestQ);
        };
    }
}