namespace ServiceInsight.Tests
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;
    using ServiceInsight.Explorer.EndpointExplorer;
    using ServiceInsight.Framework;
    using ServiceInsight.MessageList;
    using ServiceInsight.MessageProperties;
    using ServiceInsight.Models;
    using ServiceInsight.Search;
    using ServiceInsight.ServiceControl;
    using NSubstitute;
    using NUnit.Framework;
    using ServiceInsight.Framework.Events;
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