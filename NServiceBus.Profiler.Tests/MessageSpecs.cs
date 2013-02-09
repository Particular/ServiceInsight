using System;
using System.Linq;
using System.Messaging;
using Machine.Specifications;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core;

namespace NServiceBus.Profiler.Tests.Messages
{
    [Subject("messages")]
    public class with_an_actual_queue
    {
        protected static string QueueName = "TestQueue";
        protected static Queue Destination;
        protected static QueueManager Manager;
        protected static object FirstMsg;
        protected static object SecondMsg;
        protected static MessageBody FetchedMsg;
        protected static MessageQueue mq;

        Establish context = () =>
        {
            FirstMsg = "This is a test message";
            SecondMsg = "This is a second message";
            Manager = new QueueManager();
            Destination = Manager.CreatePrivateQueue(new Queue(new Address(Environment.MachineName, QueueName)));

            Manager.SendMessage(Destination, FirstMsg);
            Manager.SendMessage(Destination, SecondMsg);
        };
    }

    [Subject("messages")]
    public class when_getting_messages_from_the_queue : with_an_actual_queue
    {
        Because of = () =>
        {
            var message = Manager.GetMessages(Destination).First();
            FetchedMsg = Manager.GetMessageBody(Destination, message.Id);
        };

        It should_get_message_label = () => FetchedMsg.Label.ShouldNotBeNull();

        It should_get_message_identifiers = () =>
        {
            FetchedMsg.Id.ShouldNotBeNull();
            FetchedMsg.Id.ShouldNotBeEmpty();
        };

        It should_get_message_payload = () =>
        {
            FetchedMsg.BodyRaw.ShouldNotBeNull();
            FetchedMsg.BodyRaw.Length.ShouldBeGreaterThan(0);
        };

        It should_get_message_headers = () => { };
        It should_decode_message_as_text = () => { };
        It should_decode_message_as_xml = () => { };

        Cleanup after = () => Manager.DeleteQueue(Destination);
    }

    [Subject("messages")]
    public class when_purging_the_queue : with_an_actual_queue
    {
        Because of = () => Manager.Purge(Destination);

        It should_have_no_messages = () => Manager.GetMessages(Destination).ShouldBeEmpty();

        Cleanup after = () => Manager.DeleteQueue(Destination);
    }

    [Subject("messages")]
    public class when_deleting_messages_from_the_queue : with_an_actual_queue
    {
        Because of = () =>
        {
            var message = Manager.GetMessages(Destination).First();
            FetchedMsg = Manager.GetMessageBody(Destination, message.Id);
            Manager.DeleteMessage(Destination, FetchedMsg);
        };

        It should_remove_the_message_from_the_queue = () => Manager.GetMessages(Destination).Count.ShouldEqual(1);

        Cleanup after = () => Manager.DeleteQueue(Destination);
    }
}