namespace Particular.ServiceInsight.Tests
{
    using Caliburn.Micro;
    using Desktop.Explorer.EndpointExplorer;
    using Desktop.Framework;
    using Desktop.MessageFlow;
    using Desktop.MessageList;
    using Desktop.Models;
    using Desktop.Search;
    using Desktop.ServiceControl;
    using NSubstitute;
    using NUnit.Framework;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using Particular.ServiceInsight.Desktop.Framework.Settings;
    using Particular.ServiceInsight.Desktop.Framework.UI.ScreenManager;

    [TestFixture]
    public class MessageFlowViewModelTests
    {
        IServiceControl serviceControl;
        IEventAggregator eventAggregator;
        WindowManagerEx windowManager;
        SearchBarViewModel searchBar;
        MessageListViewModel messageList;
        ISettingsProvider settingProvider;
        EndpointExplorerViewModel endpointExplorer;
        IClipboard clipboard;

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
            serviceControl = Substitute.For<IServiceControl>();
            eventAggregator = Substitute.For<IEventAggregator>();
            clipboard = Substitute.For<IClipboard>();
            windowManager = Substitute.For<WindowManagerEx>();
            searchBar = Substitute.For<SearchBarViewModel>();
            messageList = Substitute.For<MessageListViewModel>();
            settingProvider = Substitute.For<ISettingsProvider>();
            endpointExplorer = Substitute.For<EndpointExplorerViewModel>();

            return new MessageFlowViewModel(serviceControl,
                                            eventAggregator,
                                            windowManager,
                                            searchBar,
                                            messageList,
                                            () => Substitute.For<ExceptionDetailViewModel>(),
                                            settingProvider,
                                            endpointExplorer,
                                            clipboard);
        }
    }
}