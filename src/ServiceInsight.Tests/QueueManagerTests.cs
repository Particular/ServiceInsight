namespace Particular.ServiceInsight.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Desktop.Core;
    using Desktop.Models;
    using NSubstitute;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class QueueManagerTests
    {
        private QueueManager QueueManager;
        private IQueueOperations QueueOperations;
        private IList<Queue> QueueList;
        private IList<MessageInfo> Messages;

        [SetUp]
        public void TestInitialize()
        {
            QueueList = new List<Queue>(new[] { new Queue("localhost", "queue"), new Queue("localhost", "test") });
            Messages = new List<MessageInfo>(new[] { new MessageInfo(), new MessageInfo() });

            QueueOperations = Substitute.For<IQueueOperations>();
            QueueOperations.GetQueues(Arg.Any<string>()).Returns(QueueList);
            QueueOperations.GetMessages(Arg.Any<Queue>()).Returns(Messages);

            QueueManager = new QueueManager(QueueOperations);
        }

        [Test]
        public void should_load_queues_when_initialized()
        {
            var fetchedQueues = QueueManager.GetQueues();

            fetchedQueues.ShouldNotBe(null);
            fetchedQueues.Count.ShouldBe(2);
        }

        [Test]
        public void should_create_private_queue_on_the_queue_subsystem()
        {
            var address = new Address("NewMachine", "NewQueue");
            var queue = new Queue(address);
            QueueManager.CreatePrivateQueue(queue);

            QueueOperations.Received().CreateQueue(Arg.Is<Queue>(x => x.Address.ToString().Equals("newqueue@newmachine")), Arg.Is(true));
        }

        [Test]
        public void should_remove_all_messages_from_the_queue_subsystem_when_purging_the_queue()
        {
            var fetchedQueues = QueueManager.GetQueues();
            var operatingQueue = fetchedQueues.First();
            
            QueueManager.Purge(operatingQueue);

            QueueOperations.Received().PurgeQueue(Arg.Is(operatingQueue));
        }

        [Test]
        public void should_fetch_messages_from_the_queue_subsystem()
        {
            QueueManager.GetMessages(QueueList.First());

            QueueOperations.Received().GetMessages(Arg.Any<Queue>());
        }

        [Test]
        public void should_remove_the_message_from_queue_subsystem_when_deleting_a_message()
        {
            var operatingQueue = QueueList.First();
            var messageToDelete = QueueManager.GetMessages(operatingQueue).First();

            QueueManager.DeleteMessage(operatingQueue, messageToDelete);

            QueueOperations.Received().DeleteMessage(operatingQueue, messageToDelete.Id);
        }
    }
}