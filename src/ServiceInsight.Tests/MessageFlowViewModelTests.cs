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
        DefaultServiceControl serviceControl;
        IEventAggregator eventAggregator;
        IClipboard clipboard;
        IWindowManagerEx windowManager;
        ScreenFactory screenFactory;
        SearchBarViewModel searchBar;
        IMessageListViewModel messageList;
        ISettingsProvider settingProvider;
        IEndpointExplorerViewModel endpointExplorer;

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
            clipboard = Substitute.For<IClipboard>();
            windowManager = Substitute.For<IWindowManagerEx>();
            screenFactory = Substitute.For<ScreenFactory>();
            searchBar = Substitute.For<SearchBarViewModel>();
            messageList = Substitute.For<IMessageListViewModel>();
            settingProvider = Substitute.For<ISettingsProvider>();
            endpointExplorer = Substitute.For<IEndpointExplorerViewModel>();

            return new MessageFlowViewModel(serviceControl, 
                                            eventAggregator, 
                                            clipboard, 
                                            windowManager,
                                            screenFactory, 
                                            searchBar,
                                            messageList,
                                            settingProvider,
                                            endpointExplorer);
        }

    }
}