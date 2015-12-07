namespace Particular.ServiceInsight.Tests
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;
    using Desktop.Explorer.EndpointExplorer;
    using Desktop.Framework;
    using Desktop.MessageList;
    using Desktop.MessageProperties;
    using Desktop.Models;
    using Desktop.Search;
    using Desktop.ServiceControl;
    using NSubstitute;
    using NUnit.Framework;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using Shouldly;

    [TestFixture]
    public class MessageListViewModelTests
    {
        IEventAggregator EventAggregator;
        IServiceControl ServiceControl;
        SearchBarViewModel SearchBar;
        Func<MessageListViewModel> MessageListFunc;
        IClipboard Clipboard;

        [SetUp]
        public void TestInitialize()
        {
            EventAggregator = Substitute.For<IEventAggregator>();
            ServiceControl = Substitute.For<IServiceControl>();
            SearchBar = Substitute.For<SearchBarViewModel>();
            Clipboard = Substitute.For<IClipboard>();
            MessageListFunc = () => new MessageListViewModel(EventAggregator,
                                                   ServiceControl,
                                                   SearchBar,
                                                   Substitute.For<GeneralHeaderViewModel>(),
                                                   Substitute.For<MessageSelectionContext>(),
                                                   Clipboard);
        }

        [Test]
        public void should_load_the_messages_from_the_endpoint()
        {
            var endpoint = new Endpoint { Host = "localhost", Name = "Service" };
            ServiceControl.GetAuditMessages(Arg.Is(endpoint), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<bool>())
                             .Returns(x => new PagedResult<StoredMessage>
                             {
                                 CurrentPage = 1,
                                 TotalCount = 100,
                                 Result = new List<StoredMessage>
                                 {
                                     new StoredMessage(),
                                     new StoredMessage()
                                 }
                             });

            var messageList = MessageListFunc();

            messageList.Handle(new SelectedExplorerItemChanged(new AuditEndpointExplorerItem(endpoint)));

            EventAggregator.Received(1).Publish(Arg.Any<WorkStarted>());
            EventAggregator.Received(1).Publish(Arg.Any<WorkFinished>());
            messageList.Rows.Count.ShouldBe(2);
            SearchBar.IsVisible.ShouldBe(true);
        }
    }
}