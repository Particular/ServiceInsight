namespace ServiceInsight.Tests.Framework.Commands
{
    using Caliburn.Micro;
    using NSubstitute;
    using NUnit.Framework;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.Commands;
    using ServiceInsight.Models;
    using ServiceInsight.ServiceControl;
    using System.Threading.Tasks;

    [TestFixture]
    public class RetryMessageCommandTests
    {
        IEventAggregator eventAggregator;
        IWorkNotifier workNotifier;
        IServiceControl serviceControl;
        RetryMessageCommand command;

        [SetUp]
        public void TestInitialize()
        {
            eventAggregator = Substitute.For<IEventAggregator>();
            workNotifier = Substitute.For<IWorkNotifier>();
            serviceControl = Substitute.For<IServiceControl>();
            command = new RetryMessageCommand(eventAggregator, workNotifier, serviceControl);
        }

        [Test]
        public async Task Should_use_instance_id_if_present()
        {
            command.Execute(new StoredMessage { InstanceId = "instanceId", Id = "messageId" });

            await serviceControl.Received().RetryMessage("messageId", "instanceId");
        }

        [Test]
        public async Task Should_pass_null_instance_id()
        {
            command.Execute(new StoredMessage { Id = "messageId" });

            await serviceControl.Received().RetryMessage("messageId", null);
        }
    }
}