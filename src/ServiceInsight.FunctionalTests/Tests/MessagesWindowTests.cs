namespace Particular.ServiceInsight.FunctionalTests.Tests
{
    using System;
    using Desktop.Models;
    using NUnit.Framework;
    using Services;
    using Shouldly;
    using TestData;
    using TestStack.BDDfy;
    using TestStack.BDDfy.Scanners.StepScanners.Fluent;
    using UI.Parts;
    using UI.Steps;

    public class MessagesWindowTests : UITest
    {
        public ShellScreen Shell { get; set; }
        public ConnectToServiceControl ConnectToServiceControl { get; set; }
        public EndpointExplorer EndpointExplorer { get; set; }
        public MessagesWindow MessagesWindow { get; set; }
        
        [Test]
        public void Execute()
        {
            this.Given(s => s.GivenMessagesAreProcessedBySalesEndpoint())
                .And(s => s.GivenConnectedToServiceControl())
                .When(s => s.WhenCheckingMessagesForEndpoint("Sales"))
                .Then(s => s.ThenShouldSeeEndpointMessagesInMessagesWindow())
                .BDDfy("Endpoint selected and related messages are displayed");
        }

        void GivenConnectedToServiceControl()
        {
            ConnectToServiceControl.Execute();
        }

        void GivenMessagesAreProcessedBySalesEndpoint()
        {
            var messages = new[]
            {
                CreateMessage(typeof(SubmitOrder)),
                CreateMessage(typeof(CancelOrder))
            };

            var salesEndpoint = new Endpoint { Name  = "Sales" };
            
            TestDataBuilder.EndpointBuilder().WithEndpoints(salesEndpoint).Build();
            TestDataBuilder.MessageBuilder(salesEndpoint).WithMessages(messages).Build();
        }


        void WhenCheckingMessagesForEndpoint(string endpoint)
        {
            EndpointExplorer.SelectEndpoint(endpoint);
        }

        void ThenShouldSeeEndpointMessagesInMessagesWindow()
        {
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
    }
}