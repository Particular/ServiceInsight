namespace Particular.ServiceInsight.FunctionalTests.Tests.Journeys
{
    using System.Linq;
    using Desktop.Core;
    using Desktop.Models;
    using NUnit.Framework;
    using Parts;

    public class QueueManagementJourney : ProfilerTestBase
    {
        public IQueueManager QueueManager { get; set; }
        public QueueCreationDialog QueueCreationDialog { get; set; }

        [Test]
        public void Can_create_new_queues_and_see_them_in_the_queue_explorer()
        {
            var queueName = NameGenerator.GetUniqueName("MyQueue");
            var expectedAddress = new Address(queueName);

            Shell.LayoutManager.ActivateQueueExplorer();

            Shell.MainMenu.ToolsMenu.Click();
            Shell.MainMenu.CreateQueue.Click();
            
            QueueCreationDialog.Activate();
            QueueCreationDialog.QueueName.Text = queueName;
            QueueCreationDialog.Okay.Click();

            Assert.IsTrue(QueueManager.GetQueues().Any(q => q.Address == expectedAddress));
        }
    }
}