namespace Particular.ServiceInsight.Tests
{
    using Caliburn.PresentationFramework.ApplicationModel;
    using Desktop.Core.Settings;
    using Desktop.Core.UI.ScreenManager;
    using Desktop.Events;
    using Desktop.Explorer.EndpointExplorer;
    using Desktop.MessageFlow;
    using Desktop.MessageList;
    using Desktop.Models;
    using Desktop.Search;
    using Desktop.ServiceControl;
    using ExceptionHandler;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class MessageFlowViewModelTests
    {
        private IServiceControl _serviceControl;
        private IEventAggregator _eventAggregator;
        private IClipboard _clipboard;
        private IWindowManagerEx _windowManager;
        private IScreenFactory _screenFactory;
        private ISearchBarViewModel _searchBar;
        private IMessageListViewModel _messageList;
        private ISettingsProvider _settingProvider;
        private IEndpointExplorerViewModel _endpointExplorer;

        [Test]
        public void Search_message_would_set_the_search_criteria_only()
        {
            var sut = CreateSUT();
            var messageId = "SomeMessageId";
            var msg = new StoredMessage { MessageId = messageId };
            
            sut.SearchByMessageId(msg);

            _searchBar.Received(1).Search(Arg.Is(messageId), Arg.Is(false));
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

            _eventAggregator.Received(1).Publish(Arg.Is<RequestSelectingEndpoint>(m => m.Endpoint == receiving));
        }

        private MessageFlowViewModel CreateSUT()
        {
            _serviceControl = Substitute.For<IServiceControl>();
            _eventAggregator = Substitute.For<IEventAggregator>();
            _clipboard = Substitute.For<IClipboard>();
            _windowManager = Substitute.For<IWindowManagerEx>();
            _screenFactory = Substitute.For<IScreenFactory>();
            _searchBar = Substitute.For<ISearchBarViewModel>();
            _messageList = Substitute.For<IMessageListViewModel>();
            _settingProvider = Substitute.For<ISettingsProvider>();
            _endpointExplorer = Substitute.For<IEndpointExplorerViewModel>();

            return new MessageFlowViewModel(_serviceControl, 
                                            _eventAggregator, 
                                            _clipboard, 
                                            _windowManager,
                                            _screenFactory, 
                                            _searchBar,
                                            _messageList,
                                            _settingProvider,
                                            _endpointExplorer);
        }

    }
}