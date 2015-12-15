namespace ServiceInsight.FunctionalTests.Tests
{
    using ServiceInsight.Models;
    using NUnit.Framework;
    using Services;
    using TestData;
    using TestStack.BDDfy;
    using TestStack.BDDfy.Scanners.StepScanners.Fluent;
    using UI.Parts;
    using UI.Steps;

    public class FlowDiagramTests : ShellTests
    {
        public ShellScreen Shell { get; set; }
        public ConnectToServiceControl ConnectToServiceControl { get; set; }
        public EndpointExplorer EndpointExplorer { get; set; }
        public MessagesWindow MessagesWindow { get; set; }

        [Test]
        public void ShouldDrawSimpleConversationInFlowDiagram()
        {
            this.Given(s => s.ThereIsAnEndpointWithMessages())
                .When(s => s.SelectingMessageThatIsPartOfAConversation())
                .And(s => s.MessageFlowTabIsActivated())
                .Then(s => s.MessagFlowShouldBeDisplayed())
                .BDDfy();
        }

        void ThereIsAnEndpointWithMessages()
        {
            var salesEndpoint = new Endpoint { Name = "Sales" };
            var customerRelationsEndpoint = new Endpoint { Name = "CustomerRelations" };
            const string conversationId = "OrderConversationKey";

            var submitOrderMessage = TestDataBuilder.MessageBuilder()
                                                    .WithMessageType(typeof(SubmitOrder))
                                                    .WithConversationId(conversationId)
                                                    .WithSendingEndpoint(salesEndpoint)
                                                    .WithReceivingEndpoint(salesEndpoint)
                                                    .Build();
            var orderPlacedMessage = TestDataBuilder.MessageBuilder()
                                                    .WithMessageType(typeof(OrderPlaced))
                                                    .WithConversationId(conversationId)
                                                    .WithRelatedMessageId(submitOrderMessage.MessageId)
                                                    .WithSendingEndpoint(salesEndpoint)
                                                    .WithReceivingEndpoint(salesEndpoint)
                                                    .Build();
            var orderAcceptedMessage = TestDataBuilder.MessageBuilder()
                                                      .WithMessageType(typeof(OrderAccepted))
                                                      .WithConversationId(conversationId)
                                                      .WithRelatedMessageId(orderPlacedMessage.MessageId)
                                                      .WithSendingEndpoint(salesEndpoint)
                                                      .WithReceivingEndpoint(salesEndpoint)
                                                      .Build();
            var clientPreferredMessage = TestDataBuilder.MessageBuilder()
                                                        .WithMessageType(typeof(ClientBecamePreferred))
                                                        .WithConversationId(conversationId)
                                                        .WithRelatedMessageId(orderAcceptedMessage.MessageId)
                                                        .WithSendingEndpoint(salesEndpoint)
                                                        .WithReceivingEndpoint(customerRelationsEndpoint)
                                                        .Build();

            TestDataBuilder.EndpointBuilder().WithEndpoints(salesEndpoint, customerRelationsEndpoint).Build();
            TestDataBuilder.MessageListBuilder(salesEndpoint.Name).WithMessages(submitOrderMessage, orderPlacedMessage, orderAcceptedMessage).Build();
            TestDataBuilder.MessageListBuilder(customerRelationsEndpoint.Name).WithMessages(clientPreferredMessage).Build();
            TestDataBuilder.ConversationBuilder().WithMessages(submitOrderMessage, orderPlacedMessage, orderAcceptedMessage, clientPreferredMessage).Build();
        }

        void SelectingMessageThatIsPartOfAConversation()
        {
            ConnectToServiceControl.Execute();
            EndpointExplorer.SelectEndpoint("Sales");
            MessagesWindow.SelectRow(0);
        }

        void MessageFlowTabIsActivated()
        {
            Shell.LayoutManager.SelectFlowDiagramTab();
        }

        void MessagFlowShouldBeDisplayed()
        {
            //TODO: capture image of the diagram to run approval tests on
        }
    }
}