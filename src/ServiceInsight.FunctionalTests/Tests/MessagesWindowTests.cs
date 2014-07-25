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

        const string WhenCheckingMessagesForEndpointTemplate = "When checking messages for '{0}' endpoint";
        const string ThenShouldSeeEndpointMessagesInMessagesWindowTemplate = "Then should see {0} message(s) in Messages Window";

        [Test]
        public void EndpointSelectedAndMessagesWithDetailsAreDisplayed()
        {
            this.Given(s => s.GivenSalesAndCustomerRelationEndpointsExists())
                .And(s => s.GivenMessagesAreSentToTheEndpoint())
                .And(s => s.GivenConnectedToServiceControl())
                .When(s => s.WhenCheckingMessagesForEndpoint("Sales"), WhenCheckingMessagesForEndpointTemplate)
                .Then(s => s.ThenShouldSeeEndpointMessagesInMessagesWindow(2), ThenShouldSeeEndpointMessagesInMessagesWindowTemplate)
                .And(s => s.ShouldSeeSuccessfulMessage())
                .And(s => s.ShouldSeeFailedMessage())
                .BDDfy();
        }

        [Test]
        public void EndpointsWithoutAnyMessageSelectedAndNoMessageDisplayed()
        {
            this.Given(s => s.GivenSalesAndCustomerRelationEndpointsExists())
                .And(s => s.GivenConnectedToServiceControl())
                .When(s => s.WhenCheckingMessagesForEndpoint("CustomerRelations"), WhenCheckingMessagesForEndpointTemplate)
                .Then(s => s.ThenShouldSeeEndpointMessagesInMessagesWindow(0), ThenShouldSeeEndpointMessagesInMessagesWindowTemplate)
                .BDDfy();
        }

        void GivenConnectedToServiceControl()
        {
            ConnectToServiceControl.Execute();
        }

        void GivenSalesAndCustomerRelationEndpointsExists()
        {
            TestDataBuilder.EndpointBuilder().WithEndpoints("Sales", "CustomerRelations").Build();
        }

        void GivenMessagesAreSentToTheEndpoint()
        {
            TestDataBuilder.MessageBuilder("Sales")
                           .WithMessages(CreateMessage(typeof(SubmitOrder)), 
                                         CreateMessage(typeof(CancelOrder), MessageStatus.Failed))
                           .Build();
        }

        void ShouldSeeSuccessfulMessage()
        {
            MessagesWindow.GetCellValue(0, MessagesWindow.Columns.MessageId).ShouldNotBeEmpty();
            MessagesWindow.GetCellValue(0, MessagesWindow.Columns.Status).ShouldBe("Success");
            MessagesWindow.GetCellValue(0, MessagesWindow.Columns.MessageType).ShouldBe("SubmitOrder");
            MessagesWindow.GetCellValue(0, MessagesWindow.Columns.CriticalTime).ShouldBe("5s");
            MessagesWindow.GetCellValue(0, MessagesWindow.Columns.DeliveryTime).ShouldBe("4s");
            MessagesWindow.GetCellValue(0, MessagesWindow.Columns.ProcessingTime).ShouldBe("3s");
        }

        void ShouldSeeFailedMessage()
        {
            MessagesWindow.GetCellValue(1, MessagesWindow.Columns.MessageId).ShouldNotBeEmpty();
            MessagesWindow.GetCellValue(1, MessagesWindow.Columns.Status).ShouldBe("Failed");
            MessagesWindow.GetCellValue(1, MessagesWindow.Columns.MessageType).ShouldBe("CancelOrder");
            MessagesWindow.GetCellValue(1, MessagesWindow.Columns.CriticalTime).ShouldBe("5s");
            MessagesWindow.GetCellValue(1, MessagesWindow.Columns.DeliveryTime).ShouldBe("4s");
            MessagesWindow.GetCellValue(1, MessagesWindow.Columns.ProcessingTime).ShouldBe("3s");
        }

        void WhenCheckingMessagesForEndpoint(string endpoint)
        {
            EndpointExplorer.SelectEndpoint(endpoint);
        }

        void ThenShouldSeeEndpointMessagesInMessagesWindow(int count)
        {
            MessagesWindow.GetMessageCount().ShouldBe(count);
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