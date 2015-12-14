namespace Particular.ServiceInsight.Tests
{
    using System;
    using Caliburn.Micro;
    using Microsoft.Reactive.Testing;
    using NSubstitute;
    using NUnit.Framework;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using Particular.ServiceInsight.Desktop.MessageList;
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
        MessageSelectionContext Selection;

        [SetUp]
        public void TestInitialize()
        {
            EventAggregator = Substitute.For<IEventAggregator>();
            ServiceControl = Substitute.For<IServiceControl>();
            HexContent = Substitute.For<HexContentViewModel>();
            JsonContent = Substitute.For<JsonMessageViewModel>();
            XmlContent = Substitute.For<XmlMessageViewModel>();
            Selection = new MessageSelectionContext(EventAggregator);

            MessageBodyFunc = () => new MessageBodyViewModel(HexContent, JsonContent, XmlContent, ServiceControl, EventAggregator, Selection);
        }

        [Test]
        public void Should_load_body_content_when_body_tab_is_already_selected()
        {
            new TestScheduler().With(sched =>
            {
                const string uri = "http://localhost:3333/api/somemessageid/body";

                var messageBody = MessageBodyFunc();

                messageBody.Handle(new BodyTabSelectionChanged(true));

                Selection.SelectedMessage = new StoredMessage { BodyUrl = uri };

                messageBody.Handle(new SelectedMessageChanged());

                sched.AdvanceByMs(500);

                ServiceControl.Received(1).LoadBody(Selection.SelectedMessage);
            });
        }

        [Test]
        public void Should_load_body_content_when_body_tab_is_focused()
        {
            const string uri = "http://localhost:3333/api/somemessageid/body";

            var messageBody = MessageBodyFunc();

            Selection.SelectedMessage = new StoredMessage { BodyUrl = uri };
            messageBody.Handle(new SelectedMessageChanged());

            messageBody.Handle(new BodyTabSelectionChanged(true));


            ServiceControl.Received(1).LoadBody(Selection.SelectedMessage);
        }

        [Test]
        public void Should_not_load_body_content_when_body_tab_is_not_focused()
        {
            const string uri = "http://localhost:3333/api/somemessageid/body";

            var messageBody = MessageBodyFunc();

            messageBody.Handle(new BodyTabSelectionChanged(false));

            Selection.SelectedMessage = new StoredMessage { BodyUrl = uri };

            messageBody.Handle(new SelectedMessageChanged());

            ServiceControl.Received(0).LoadBody(Selection.SelectedMessage);
        }

        [Test]
        public void Should_not_load_body_content_when_selected_message_has_no_body_url()
        {
            var messageBody = MessageBodyFunc();

            messageBody.Handle(new BodyTabSelectionChanged(true));

            Selection.SelectedMessage = new StoredMessage { BodyUrl = null };

            messageBody.Handle(new SelectedMessageChanged());

            ServiceControl.Received(0).LoadBody(Selection.SelectedMessage);
        }
    }
}