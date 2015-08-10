namespace Particular.ServiceInsight.Tests
{
    using System;
    using Caliburn.Micro;
    using Microsoft.Reactive.Testing;
    using NSubstitute;
    using NUnit.Framework;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using Particular.ServiceInsight.Desktop.MessageViewers;
    using Particular.ServiceInsight.Desktop.MessageViewers.HexViewer;
    using Particular.ServiceInsight.Desktop.MessageViewers.JsonViewer;
    using Particular.ServiceInsight.Desktop.MessageViewers.XmlViewer;
    using Particular.ServiceInsight.Desktop.Models;
    using Particular.ServiceInsight.Desktop.ServiceControl;
    using ReactiveUI.Testing;

    [TestFixture]
    public class MessageBodyViewModelTests
    {
        IEventAggregator EventAggregator;
        IServiceControl ServiceControl;
        HexContentViewModel HexContent;
        JsonMessageViewModel JsonContent;
        XmlMessageViewModel XmlContent;
        Func<MessageBodyViewModel> MessageBodyFunc;

        [SetUp]
        public void TestInitialize()
        {
            EventAggregator = Substitute.For<IEventAggregator>();
            ServiceControl = Substitute.For<IServiceControl>();
            HexContent = Substitute.For<HexContentViewModel>();
            JsonContent = Substitute.For<JsonMessageViewModel>();
            XmlContent = Substitute.For<XmlMessageViewModel>();
            MessageBodyFunc = () => new MessageBodyViewModel(HexContent, JsonContent, XmlContent, ServiceControl, EventAggregator);
        }

        [Test]
        public void Should_load_body_content_when_body_tab_is_already_selected()
        {
            new TestScheduler().With(sched =>
            {
                const string uri = "http://localhost:3333/api/somemessageid/body";

                var messageBody = MessageBodyFunc();

                messageBody.Handle(new BodyTabSelectionChanged(true));

                messageBody.Handle(new SelectedMessageChanged(new StoredMessage { BodyUrl = uri }));

                sched.AdvanceByMs(500);

                ServiceControl.Received(1).LoadBody(messageBody.SelectedMessage);
            });
        }

        [Test]
        public void Should_load_body_content_when_body_tab_is_focused()
        {
            const string uri = "http://localhost:3333/api/somemessageid/body";

            var messageList = MessageBodyFunc();

            messageList.Handle(new SelectedMessageChanged(new StoredMessage { BodyUrl = uri }));

            messageList.Handle(new BodyTabSelectionChanged(true));

            ServiceControl.Received(1).LoadBody(messageList.SelectedMessage);
        }

        [Test]
        public void Should_not_load_body_content_when_body_tab_is_not_focused()
        {
            const string uri = "http://localhost:3333/api/somemessageid/body";

            var messageList = MessageBodyFunc();

            messageList.Handle(new BodyTabSelectionChanged(false));

            messageList.Handle(new SelectedMessageChanged(new StoredMessage { BodyUrl = uri }));

            ServiceControl.Received(0).LoadBody(messageList.SelectedMessage);
        }

        [Test]
        public void Should_not_load_body_content_when_selected_message_has_no_body_url()
        {
            var messageList = MessageBodyFunc();

            messageList.Handle(new BodyTabSelectionChanged(true));

            messageList.Handle(new SelectedMessageChanged(new StoredMessage { BodyUrl = null }));

            ServiceControl.Received(0).LoadBody(messageList.SelectedMessage);
        }
    }
}