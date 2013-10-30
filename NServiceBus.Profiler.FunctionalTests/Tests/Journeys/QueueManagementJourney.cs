using System.Linq;
using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.Models;
using NServiceBus.Profiler.FunctionalTests.Parts;
using NUnit.Framework;
using Shouldly;

namespace NServiceBus.Profiler.FunctionalTests.Tests.Journeys
{
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