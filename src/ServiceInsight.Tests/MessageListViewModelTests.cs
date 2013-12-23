using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.PresentationFramework.ApplicationModel;
using ExceptionHandler;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer;
using NServiceBus.Profiler.Desktop.Explorer.QueueExplorer;
using NServiceBus.Profiler.Desktop.MessageList;
using NServiceBus.Profiler.Desktop.MessageProperties;
using NServiceBus.Profiler.Desktop.Models;
using NServiceBus.Profiler.Desktop.Search;
using NServiceBus.Profiler.Desktop.ServiceControl;
using NServiceBus.Profiler.Tests.Helpers;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace NServiceBus.Profiler.Tests
{
    [TestFixture]
    [Ignore] //TODO: Due to a problem with the async On...Change methods. To investigate.
    public class MessageListViewModelTests : AsyncTestBase
    {
        private IEventAggregator EventAggregator;
        private IServiceControl ServiceControl;
        private ISearchBarViewModel SearchBar;
        private Dictionary<Queue, List<MessageInfo>> MessageStore;
        private MessageListViewModel MessageList;
        private IMessageListView View;

        [SetUp]
        public void TestInitialize()
        {
            EventAggregator = Substitute.For<IEventAggregator>();
            ServiceControl = Substitute.For<IServiceControl>();
            MessageStore = new Dictionary<Queue, List<MessageInfo>>();
            SearchBar = Substitute.For<ISearchBarViewModel>();
            View = Substitute.For<IMessageListView>();
            MessageList = new MessageListViewModel(EventAggregator, 
                                                   ServiceControl, 
                                                   SearchBar, 
                                                   Substitute.For<IErrorHeaderViewModel>(), 
                                                   Substitute.For<IGeneralHeaderViewModel>(), 
                                                   Substitute.For<IClipboard>());
            MessageList.AttachView(View, null);
        }

        [Test]
        public void should_load_the_messages_from_the_endpoint()
        {
            var endpoint = new Endpoint { Machine = "localhost", Name = "Service" };
            ServiceControl.GetAuditMessages(Arg.Is(endpoint), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<bool>())
                             .Returns(x => Task.Run(() => new PagedResult<StoredMessage>
                             {
                                 CurrentPage = 1,
                                 TotalCount = 100,
                                 Result = new List<StoredMessage>
                                 {
                                     new StoredMessage(),
                                     new StoredMessage()
                                 }
                             }));

            AsyncHelper.Run(() => MessageList.Handle(new SelectedExplorerItemChanged(new AuditEndpointExplorerItem(endpoint))));

            EventAggregator.Received(1).Publish(Arg.Any<WorkStarted>());
            EventAggregator.Received(1).Publish(Arg.Any<WorkFinished>());
            MessageList.Rows.Count.ShouldBe(2);
            SearchBar.IsVisible.ShouldBe(true);
        }

        [Test]
        public void should_load_the_messages_from_the_queue()
        {
            var selectedQueue = new Queue("testqueue");
            MessageStore.Add(selectedQueue, new List<MessageInfo>(new[]
            {
                new MessageInfo { MessageType = "Contracts.SomeMessage, Contracts"}, 
                new MessageInfo { MessageType = "Contracts.SomeMessage+SubMessage, Contracts"},
                new MessageInfo { MessageType = ""}, 
            }));

            AsyncHelper.Run(() => MessageList.Handle(new SelectedExplorerItemChanged(new QueueExplorerItem(selectedQueue)))); // trigger refresh

            EventAggregator.Received(1).Publish(Arg.Any<WorkStarted>());
            EventAggregator.Received(1).Publish(Arg.Any<WorkFinished>());
            MessageList.Rows.Count.ShouldBe(3);
            SearchBar.IsVisible.ShouldBe(false);
            MessageList.Rows[0].FriendlyMessageType.ShouldBe("SomeMessage");
            MessageList.Rows[1].FriendlyMessageType.ShouldBe("SubMessage");
            MessageList.Rows[2].FriendlyMessageType.ShouldBe(null);
        }
    }
}