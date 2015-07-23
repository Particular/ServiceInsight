namespace Particular.ServiceInsight.FunctionalTests.Tests
{
    using Desktop.Models;
    using NUnit.Framework;
    using Services;
    using Shouldly;
    using TestData;
    using TestStack.BDDfy;
    using TestStack.BDDfy.Scanners.StepScanners.Fluent;
    using UI.Parts;
    using UI.Steps;

    public class MessagesWindowTests : ShellTests
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
            var orderSubmittedMessage = TestDataBuilder.MessageBuilder().WithMessageType(typeof(SubmitOrder)).Build();
            var orderCancelledMessage = TestDataBuilder.MessageBuilder().WithMessageType(typeof(CancelOrder)).WithMessageStatus(MessageStatus.Failed).Build();

            TestDataBuilder.MessageListBuilder("Sales")
                           .WithMessages(orderSubmittedMessage, orderCancelledMessage)
                           .Build();
        }

        void ShouldSeeSuccessfulMessage()
        {
            MessagesWindow.GetCellValue(0, MessagesWindow.Columns.MessageId).ShouldNotBeEmpty();
            MessagesWindow.GetCellValue(0, MessagesWindow.Columns.Status).ShouldBe("Success");
            MessagesWindow.GetCellValue(0, MessagesWindow.Columns.MessageType).ShouldBe("SubmitOrder");
            MessagesWindow.GetCellValue(0, MessagesWindow.Columns.ProcessingTime).ShouldBe("3s");
            MessagesWindow.GetCellValue(0, MessagesWindow.Columns.TimeSent).ShouldNotBeEmpty();
        }

        void ShouldSeeFailedMessage()
        {
            MessagesWindow.GetCellValue(1, MessagesWindow.Columns.MessageId).ShouldNotBeEmpty();
            MessagesWindow.GetCellValue(1, MessagesWindow.Columns.Status).ShouldBe("Failed");
            MessagesWindow.GetCellValue(1, MessagesWindow.Columns.MessageType).ShouldBe("CancelOrder");
            MessagesWindow.GetCellValue(1, MessagesWindow.Columns.ProcessingTime).ShouldBe("3s");
            MessagesWindow.GetCellValue(0, MessagesWindow.Columns.TimeSent).ShouldNotBeEmpty();
        }

        void WhenCheckingMessagesForEndpoint(string endpoint)
        {
            EndpointExplorer.SelectEndpoint(endpoint);
        }

        void ThenShouldSeeEndpointMessagesInMessagesWindow(int count)
        {
            MessagesWindow.GetMessageCount().ShouldBe(count);
        }
    }
}