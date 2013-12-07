using System;
using System.Linq;
using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.Models;
using NUnit.Framework;
using Shouldly;

namespace NServiceBus.Profiler.Tests
{
    [TestFixture]
    [Category("Integration")]
    public class QueueManagerIntegrationTests
    {
        private Queue destination;
        private QueueManager manager;
        private Queue source;
        private Queue nonTransactional;
        private const string FirstMsg = "This is a test message";
        private const string SecondMsg = "This is a second message";
        private const string QueueName = "TestQueue";
        private string SourceQueueName;

        [SetUp]
        public void TestInitialize()
        {
            SourceQueueName = Guid.NewGuid().ToString("N");
            manager = new QueueManager();
            source = manager.CreatePrivateQueue(new Queue(new Address(Environment.MachineName, SourceQueueName)));
            destination = manager.CreatePrivateQueue(new Queue(new Address(Environment.MachineName, QueueName)));
            nonTransactional = manager.CreatePrivateQueue(new Queue(new Address("Nontransactional")), transactional: false);
        }

        [Test]
        public void should_retreive_and_map_message_from_the_queue()
        {
            manager.SendMessage(destination, FirstMsg);
            manager.SendMessage(destination, SecondMsg);

            var message = manager.GetMessages(destination).First();
            
            var fetchedMsg = manager.GetMessageBody(destination, message.Id);

            fetchedMsg.Label.ShouldNotBe(null);
            fetchedMsg.Id.ShouldNotBe(null);
            fetchedMsg.Id.ShouldNotBeEmpty();
            fetchedMsg.BodyRaw.ShouldNotBe(null);
            fetchedMsg.BodyRaw.Length.ShouldBeGreaterThan(0);
        }

        [Test]
        public void should_be_able_to_purge_the_queue()
        {
            manager.SendMessage(destination, FirstMsg);
            manager.SendMessage(destination, SecondMsg);

            manager.Purge(destination);

            manager.GetMessages(destination).ShouldBeEmpty();
        }

        [Test]
        public void should_be_able_to_remove_a_mesage_from_the_queue()
        {
            manager.SendMessage(destination, FirstMsg);
            manager.SendMessage(destination, SecondMsg);

            var message = manager.GetMessages(destination).First();
            var fetchedMsg = manager.GetMessageBody(destination, message.Id);

            manager.DeleteMessage(destination, fetchedMsg);

            manager.GetMessages(destination).Count.ShouldBe(1);
        }

        [Test]
        public void should_determine_local_queue_as_local()
        {
            var queue = manager.GetQueues().First(q => q.Address.Queue == SourceQueueName);

            queue.IsRemoteQueue().ShouldBe(false);
            queue.ToString().ShouldContain(SourceQueueName);
            queue.ToString().ShouldContain(Environment.MachineName.ToLower());
        }

        [Test]
        public void should_be_able_to_move_messages_between_queues()
        {
            manager.SendMessage(source, "This is a test message");

            var message = manager.GetMessages(source).First();
            
            manager.MoveMessage(source, destination, message.Id);

            manager.GetMessages(destination).Count.ShouldBe(1);
        }

        [Test]
        public void moving_non_existing_messages_across_queues_throws()
        {
            manager.SendMessage(source, "This is a test message");

            var message = manager.GetMessages(source).First();
            manager.DeleteMessage(source, message);

            var error = Should.Throw<InvalidOperationException>(() => manager.MoveMessage(source, destination, message.Id));

            error.ShouldNotBe(null);
        }

        [Test]
        public void moving_message_to_non_transactional_queue_throws()
        {
            manager.SendMessage(source, "This is a test message");

            var message = manager.GetMessages(source).First();
            
            var error = Should.Throw<Exception>(() => manager.MoveMessage(source, nonTransactional, message.Id));

            error.ShouldNotBe(null);
            error.ShouldBeTypeOf<QueueManagerException>();
            error.Message.ShouldContain("is not transactional");
        }

        [Test]
        public void should_retreive_message_count_in_the_queue()
        {
            manager.SendMessage(source, "This is a test message");
            manager.SendMessage(source, "This the second test message");

            var messageCount = manager.GetMessageCount(source);

            messageCount.ShouldBe(2); //one already existed
        }

        [Test]
        public void recreating_an_existing_queue_will_return_existing_one_as_mapped()
        {
            var reCreatedQueue = manager.CreatePrivateQueue(source); //source already exists

            reCreatedQueue.FormatName.ShouldBe(source.FormatName);
            reCreatedQueue.Address.ShouldBe(source.Address);
            reCreatedQueue.IsTransactional.ShouldBe(source.IsTransactional);
            reCreatedQueue.CanRead.ShouldBe(source.CanRead);
        }

        [TearDown]
        public void TestCleanup()
        {
            manager.DeleteQueue(destination);
            manager.DeleteQueue(source);
            manager.DeleteQueue(nonTransactional);
        }
    }
}