using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.Models;
using NServiceBus.Profiler.Tests.Helpers;
using NUnit.Framework;
using Shouldly;

namespace NServiceBus.Profiler.Tests
{
    [TestFixture]
    [Category("Integration")]
    public class AsyncQueueOperationsTests
    {
        private IQueueManagerAsync manager;
        private Queue sourceQ;
        private Queue destinationQ;

        [TestFixtureSetUp]
        public void TestInitialize()
        {
            manager = new AsyncQueueManager(new MSMQueueOperations(new DefaultMapper()));
            sourceQ = manager.CreatePrivateQueue(new Queue("TestSource"));
            destinationQ = manager.CreatePrivateQueue(new Queue("TestDest"));
        }

        [Test]
        public void should_be_able_to_load_messages_from_the_queue()
        {
            manager = new AsyncQueueManager(new MSMQueueOperations(new DefaultMapper()));

            for (var i = 0; i < 500; i++)
            {
                manager.SendMessage(destinationQ, string.Format("Test message number {0}, this is a somewhat larger text message. this is a somewhat larger text message. this is a somewhat larger text message. this is a somewhat larger text message.", i));
            }

            var messages = AsyncHelper.Run(() => manager.GetMessages(destinationQ));
            messages.Count.ShouldBe(500);
        }

        [TestFixtureTearDown]
        public void TestCleanup()
        {
            manager.DeleteQueue(sourceQ);
            manager.DeleteQueue(destinationQ);
        }
    }
}