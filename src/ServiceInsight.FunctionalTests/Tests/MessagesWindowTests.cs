namespace Particular.ServiceInsight.FunctionalTests.Tests
{
    using System;
    using Desktop.Models;
    using NUnit.Framework;
    using Services;
    using Shouldly;
    using UI.Parts;
    using UI.Steps;

    public class MessagesWindowTests : UITest
    {
        public ShellScreen Shell { get; set; }
        public ConnectToServiceControl ConnectToServiceControl { get; set; }
        public EndpointExplorer EndpointExplorer { get; set; }
        public MessagesWindow MessagesWindow { get; set; }

        [SetUp]
        public void Initialize()
        {
            var messages = new[]
            {
                CreateMessage(typeof(SubmitOrder)),
                CreateMessage(typeof(CancelOrder))
            };

            TestDataBuilder.EndpointBuilder().Build();
            TestDataBuilder.MessageBuilder().WithMessages(messages).Build();
        }

        [Test]
        public void Shows_single_node_when_a_message_is_sent()
        {
            ConnectToServiceControl.Execute();

            MessagesWindow.GetMessageCount().ShouldBe(2);
        }

        StoredMessage CreateMessage(Type messageType, MessageStatus status = MessageStatus.Successful)
        {
            return new StoredMessage
            {
                Id = Guid.NewGuid().ToString(),
                MessageId = Guid.NewGuid().ToString(),
                MessageType = messageType.FullName,
                TimeSent = DateTime.Now,
                CriticalTime = TimeSpan.FromSeconds(5),
                DeliveryTime = TimeSpan.FromSeconds(4),
                ProcessingTime = TimeSpan.FromSeconds(3),
                Status = status
            };
        }

        public class SubmitOrder
        {
            public int OrderNumber { get; set; }
            public string[] VideoIds { get; set; }
            public string ClientId { get; set; }
            public string EncryptedCreditCardNumber { get; set; }
            public string EncryptedExpirationDate { get; set; }
        }

        public class CancelOrder
        {
            public int OrderNumber { get; set; }
            public string ClientId { get; set; }
        }
    }
}