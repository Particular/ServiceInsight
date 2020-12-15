﻿using ServiceInsight.MessageViewers.CustomMessageViewer;

namespace ServiceInsight.Tests
{
    using System;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using NSubstitute;
    using NUnit.Framework;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.MessageList;
    using ServiceInsight.MessageViewers;
    using ServiceInsight.MessageViewers.HexViewer;
    using ServiceInsight.MessageViewers.JsonViewer;
    using ServiceInsight.MessageViewers.XmlViewer;
    using ServiceInsight.Models;
    using ServiceInsight.ServiceControl;

    class TestCustomMessageViewerResolver : ICustomMessageViewerResolver
    {
        public ICustomMessageBodyViewer GetCustomMessageBodyViewer()
        {
            return new NopViewer();
        }
    }

    [TestFixture]
    public class MessageBodyViewModelTests
    {
        IEventAggregator eventAggregator;
        IWorkNotifier workNotifier;
        IServiceControl serviceControl;
        HexContentViewModel hexContent;
        JsonMessageViewModel jsonContent;
        XmlMessageViewModel xmlContent;
        Func<MessageBodyViewModel> messageBodyFunc;
        MessageSelectionContext selection;

        [SetUp]
        public void TestInitialize()
        {
            eventAggregator = Substitute.For<IEventAggregator>();
            workNotifier = Substitute.For<IWorkNotifier>();
            serviceControl = Substitute.For<IServiceControl>();
            hexContent = Substitute.For<HexContentViewModel>();
            jsonContent = Substitute.For<JsonMessageViewModel>();
            xmlContent = Substitute.For<XmlMessageViewModel>();
            selection = new MessageSelectionContext(eventAggregator);

            messageBodyFunc = () => new MessageBodyViewModel(hexContent, jsonContent, xmlContent, new TestCustomMessageViewerResolver(), serviceControl, workNotifier, selection);
        }

        [Test]
        public async Task Should_load_body_content_when_body_tab_is_already_selected()
        {
            const string uri = "http://localhost:3333/api/somemessageid/body";

            var messageBody = messageBodyFunc();

            await messageBody.Handle(new BodyTabSelectionChanged(true));

            selection.SelectedMessage = new StoredMessage { BodyUrl = uri };

            await messageBody.Handle(new SelectedMessageChanged());

            await serviceControl.Received(1).LoadBody(selection.SelectedMessage);
        }

        [Test]
        public async Task Should_load_body_content_when_body_tab_is_focused()
        {
            const string uri = "http://localhost:3333/api/somemessageid/body";

            var messageBody = messageBodyFunc();

            selection.SelectedMessage = new StoredMessage { BodyUrl = uri };
            await messageBody.Handle(new SelectedMessageChanged());

            await messageBody.Handle(new BodyTabSelectionChanged(true));

            await serviceControl.Received(1).LoadBody(selection.SelectedMessage);
        }

        [Test]
        public async Task Should_not_load_body_content_when_body_tab_is_not_focused()
        {
            const string uri = "http://localhost:3333/api/somemessageid/body";

            var messageBody = messageBodyFunc();

            await messageBody.Handle(new BodyTabSelectionChanged(false));

            selection.SelectedMessage = new StoredMessage { BodyUrl = uri };

            await messageBody.Handle(new SelectedMessageChanged());

            await serviceControl.Received(0).LoadBody(selection.SelectedMessage);
        }

        [Test]
        public async Task Should_not_load_body_content_when_selected_message_has_no_body_url()
        {
            var messageBody = messageBodyFunc();

            await messageBody.Handle(new BodyTabSelectionChanged(true));

            selection.SelectedMessage = new StoredMessage { BodyUrl = null };

            await messageBody.Handle(new SelectedMessageChanged());

            await serviceControl.Received(0).LoadBody(selection.SelectedMessage);
        }
    }
}