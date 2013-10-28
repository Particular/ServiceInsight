using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.PresentationFramework.ApplicationModel;
using ExceptionHandler;
using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer;
using NServiceBus.Profiler.Desktop.Explorer.QueueExplorer;
using NServiceBus.Profiler.Desktop.Management;
using NServiceBus.Profiler.Desktop.MessageList;
using NServiceBus.Profiler.Desktop.MessageProperties;
using NServiceBus.Profiler.Desktop.Models;
using NServiceBus.Profiler.Desktop.ScreenManager;
using NServiceBus.Profiler.Desktop.Search;
using NServiceBus.Profiler.Desktop.Shell;
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
        private IQueueManagerAsync QueueManager;
        private IWindowManagerEx WindowManager;
        private IEventAggregator EventAggregator;
        private IManagementService ManagementService;
        private ISearchBarViewModel SearchBar;
        private IStatusBarManager StatusBarManager;
        private Dictionary<Queue, List<MessageInfo>> MessageStore;
        private MessageListViewModel MessageList;
        private IMessageListView View;

        [SetUp]
        public void TestInitialize()
        {
            EventAggregator = Substitute.For<IEventAggregator>();
            ManagementService = Substitute.For<IManagementService>();
            MessageStore = new Dictionary<Queue, List<MessageInfo>>();
            QueueManager = new FakeQueueManager(MessageStore);
            WindowManager = Substitute.For<IWindowManagerEx>();
            SearchBar = Substitute.For<ISearchBarViewModel>();
            StatusBarManager = Substitute.For<IStatusBarManager>();
            View = Substitute.For<IMessageListView>();
            MessageList = new MessageListViewModel(EventAggregator, WindowManager, ManagementService, 
                                                   QueueManager, SearchBar, 
                                                   Substitute.For<IErrorHeaderViewModel>(), 
                                                   Substitute.For<IGeneralHeaderViewModel>(), 
                                                   Substitute.For<IClipboard>(), 
                                                   StatusBarManager);
            MessageList.AttachView(View, null);
        }

        [Test]
        public void should_load_the_messages_from_the_endpoint()
        {
            var endpoint = new Endpoint { Machine = "localhost", Name = "Service" };
            ManagementService.GetAuditMessages(Arg.Is(endpoint), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<bool>())
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
            MessageList.Messages.Count.ShouldBe(2);
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
            MessageList.Messages.Count.ShouldBe(3);
            SearchBar.IsVisible.ShouldBe(false);
            MessageList.Messages[0].FriendlyMessageType.ShouldBe("SomeMessage");
            MessageList.Messages[1].FriendlyMessageType.ShouldBe("SubMessage");
            MessageList.Messages[2].FriendlyMessageType.ShouldBe(null);
        }
    }
}