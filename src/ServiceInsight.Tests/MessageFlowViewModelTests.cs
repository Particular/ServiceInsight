namespace Particular.ServiceInsight.Tests
{
    using Caliburn.Micro;
    using Desktop.Core.Settings;
    using Desktop.Core.UI.ScreenManager;
    using Desktop.Events;
    using Desktop.Explorer.EndpointExplorer;
    using Desktop.Framework;
    using Desktop.MessageFlow;
    using Desktop.MessageList;
    using Desktop.Models;
    using Desktop.Search;
    using Desktop.ServiceControl;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class MessageFlowViewModelTests
    {
        DefaultServiceControl serviceControl;
        IEventAggregator eventAggregator;
        WindowManagerEx windowManager;
        ScreenFactory screenFactory;
        SearchBarViewModel searchBar;
        MessageListViewModel messageList;
        ISettingsProvider settingProvider;
        EndpointExplorerViewModel endpointExplorer;

        [Test]
        public void Search_message_would_set_the_search_criteria_only()
        {
            var sut = CreateSUT();
            var messageId = "SomeMessageId";
            var msg = new StoredMessage { MessageId = messageId };

            sut.SearchByMessageId(msg);

            searchBar.Received(1).Search(Arg.Is(messageId), Arg.Is(false));
        }

        [Test]
        public void Search_message_would_set_the_endpoint_name()
        {
            var sut = CreateSUT();
            var messageId = "SomeMessageId";
            var receiving = new Endpoint();
            var sending = new Endpoint();
            var msg = new StoredMessage
            {
                MessageId = messageId,
                ReceivingEndpoint = receiving,
                SendingEndpoint = sending,
            };

            sut.SearchByMessageId(msg);

            eventAggregator.Received(1).Publish(Arg.Is<RequestSelectingEndpoint>(m => m.Endpoint == receiving));
        }

        MessageFlowViewModel CreateSUT()
        {
            serviceControl = Substitute.For<DefaultServiceControl>();
            eventAggregator = Substitute.For<IEventAggregator>();
            AppServices.Clipboard = Substitute.For<IClipboard>();
            windowManager = Substitute.For<WindowManagerEx>();
            screenFactory = Substitute.For<ScreenFactory>();
            searchBar = Substitute.For<SearchBarViewModel>();
            messageList = Substitute.For<MessageListViewModel>();
            settingProvider = Substitute.For<ISettingsProvider>();
            endpointExplorer = Substitute.For<EndpointExplorerViewModel>();

            return new MessageFlowViewModel(serviceControl,
                                            eventAggregator,
                                            windowManager,
                                            screenFactory,
                                            searchBar,
                                            messageList,
                                            settingProvider,
                                            endpointExplorer);
        }
    }
}