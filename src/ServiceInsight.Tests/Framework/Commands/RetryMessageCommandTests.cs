namespace ServiceInsight.Tests.Framework.Commands
{
    using Caliburn.Micro;
    using NSubstitute;
    using NUnit.Framework;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.Commands;
    using ServiceInsight.Models;
    using ServiceInsight.ServiceControl;

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
        public void Should_use_instance_id_if_present()
        {
            command.Execute(new StoredMessage { InstanceId = "instanceId", Id = "messageId" });

            serviceControl.Received().RetryMessage("messageId", "instanceId");
        }

        [Test]
        public void Should_pass_null_instance_id()
        {
            command.Execute(new StoredMessage { Id = "messageId" });

            serviceControl.Received().RetryMessage("messageId", null);
        }
    }
}