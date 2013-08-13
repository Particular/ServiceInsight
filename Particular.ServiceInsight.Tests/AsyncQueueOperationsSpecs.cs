using Machine.Specifications;
using Particular.ServiceInsight.Desktop.Core;
using Particular.ServiceInsight.Desktop.Models;
using Particular.ServiceInsight.Tests.Helpers;

namespace Particular.ServiceInsight.Tests.Messages
{
    [Subject("queues")]
    [Ignore("To create test messages for perf measurements")]
    public class with_messages_in_the_queue
    {
        protected static Queue SourceQ;
        protected static Queue DestinationQ;
        protected static IQueueManagerAsync Manager;
        protected static int MessageCount;

        Establish context = () =>
        {
            Manager = new AsyncQueueManager(new MSMQueueOperations());
            SourceQ = Manager.CreatePrivateQueue(new Queue("TestSource"));
            DestinationQ = Manager.CreatePrivateQueue(new Queue("TestDest"));
            for (var i = 0; i < 500; i++)
            {
                Manager.SendMessage(DestinationQ, string.Format("Test message number {0}, this is a somewhat larger text message. this is a somewhat larger text message. this is a somewhat larger text message. this is a somewhat larger text message.", i));
            }
        };

        Because of = () =>
        {
            var messages = AsyncHelper.Run(() => Manager.GetMessages(DestinationQ));
            MessageCount = messages.Count;
        };

        It should_be_able_to_load_messages_from_the_queue = () => MessageCount.ShouldEqual(500);

        Cleanup after = () =>
        {
            Manager.DeleteQueue(SourceQ);
            Manager.DeleteQueue(DestinationQ);
        };
    }
}