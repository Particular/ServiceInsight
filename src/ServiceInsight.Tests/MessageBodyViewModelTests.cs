namespace ServiceInsight.Tests
{
    using System;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using NSubstitute;
    using NUnit.Framework;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.Events;
    using MessageList;
    using MessageViewers;
    using MessageViewers.HexViewer;
    using MessageViewers.JsonViewer;
    using MessageViewers.XmlViewer;
    using Models;
    using ServiceControl;
    using MessageViewers.CustomMessageViewer;
    using Explorer.EndpointExplorer;

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
        ServiceControlClientRegistry clientRegistry;
        TestCustomMessageViewerResolver resolver;

        string baseUrl = "http://localhost:3333/api/";

        [SetUp]
        public void TestInitialize()
        {
            eventAggregator = Substitute.For<IEventAggregator>();
            workNotifier = Substitute.For<IWorkNotifier>();
            serviceControl = Substitute.For<IServiceControl>();
            hexContent = Substitute.For<HexContentViewModel>();
            jsonContent = Substitute.For<JsonMessageViewModel>();
            xmlContent = Substitute.For<XmlMessageViewModel>();
            clientRegistry = Substitute.For<ServiceControlClientRegistry>();
            resolver = new TestCustomMessageViewerResolver();
            selection = new MessageSelectionContext(eventAggregator);

            clientRegistry.GetServiceControl(Arg.Any<string>()).Returns(serviceControl);
            messageBodyFunc = () => new MessageBodyViewModel(hexContent, jsonContent, xmlContent, resolver, serviceControl, workNotifier, selection, clientRegistry);
        }

        [Test]
        public async Task Should_load_body_content_when_body_tab_is_already_selected()
        {
            string uri = baseUrl + "somemessageid/body";

            var messageBody = messageBodyFunc();

            await messageBody.Handle(new BodyTabSelectionChanged(true));

            selection.SelectedMessage = new StoredMessage { BodyUrl = uri };

            messageBody.Handle(new SelectedExplorerItemChanged(new ServiceControlExplorerItem(baseUrl)));

            await messageBody.Handle(new SelectedMessageChanged());

            await serviceControl.Received(1).LoadBody(selection.SelectedMessage);
        }

        [Test]
        public async Task Should_load_body_content_when_body_tab_is_focused()
        {
            string uri = baseUrl + "somemessageid/body";

            var messageBody = messageBodyFunc();

            selection.SelectedMessage = new StoredMessage { BodyUrl = uri };
            await messageBody.Handle(new SelectedMessageChanged());

            messageBody.Handle(new SelectedExplorerItemChanged(new ServiceControlExplorerItem(baseUrl)));

            await messageBody.Handle(new BodyTabSelectionChanged(true));

            await serviceControl.Received(1).LoadBody(selection.SelectedMessage);
        }

        [Test]
        public async Task Should_not_load_body_content_when_body_tab_is_not_focused()
        {
            string uri = baseUrl + "somemessageid/body";

            var messageBody = messageBodyFunc();

            await messageBody.Handle(new BodyTabSelectionChanged(false));

            selection.SelectedMessage = new StoredMessage { BodyUrl = uri };

            messageBody.Handle(new SelectedExplorerItemChanged(new ServiceControlExplorerItem(baseUrl)));

            await messageBody.Handle(new SelectedMessageChanged());

            await serviceControl.Received(0).LoadBody(selection.SelectedMessage);
        }

        [Test]
        public async Task Should_not_load_body_content_when_selected_message_has_no_body_url()
        {
            var messageBody = messageBodyFunc();

            await messageBody.Handle(new BodyTabSelectionChanged(true));

            messageBody.Handle(new SelectedExplorerItemChanged(new ServiceControlExplorerItem(baseUrl)));

            selection.SelectedMessage = new StoredMessage { BodyUrl = null };

            await messageBody.Handle(new SelectedMessageChanged());

            await serviceControl.Received(0).LoadBody(selection.SelectedMessage);
        }
    }
}