namespace Particular.ServiceInsight.Tests
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;
    using Desktop.Events;
    using Desktop.Explorer.EndpointExplorer;
    using Desktop.Framework;
    using Desktop.MessageList;
    using Desktop.MessageProperties;
    using Desktop.Models;
    using Desktop.Search;
    using Desktop.ServiceControl;
    using Microsoft.Reactive.Testing;
    using NSubstitute;
    using NUnit.Framework;
    using ReactiveUI.Testing;
    using Shouldly;

    [TestFixture]
    public class MessageListViewModelTests
    {
        IEventAggregator EventAggregator;
        DefaultServiceControl ServiceControl;
        SearchBarViewModel SearchBar;
        Func<MessageListViewModel> MessageListFunc;
        IClipboard Clipboard;

        [SetUp]
        public void TestInitialize()
        {
            EventAggregator = Substitute.For<IEventAggregator>();
            ServiceControl = Substitute.For<DefaultServiceControl>();
            SearchBar = Substitute.For<SearchBarViewModel>();
            Clipboard = Substitute.For<IClipboard>();
            MessageListFunc = () => new MessageListViewModel(EventAggregator,
                                                   ServiceControl,
                                                   SearchBar,
                                                   Substitute.For<GeneralHeaderViewModel>(),
                                                   Clipboard);
        }

        [Test]
        public void should_load_the_messages_from_the_endpoint()
        {
            var endpoint = new Endpoint { HostDisplayName = "localhost", Name = "Service" };
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

        [Test]
        public void Should_load_body_content_when_body_tab_is_already_selected()
        {
            new TestScheduler().With(sched =>
            {
                const string uri = "http://localhost:3333/api/somemessageid/body";

                var messageList = MessageListFunc();

                messageList.FocusedRow = null;
                messageList.Handle(new BodyTabSelectionChanged(true));

                messageList.FocusedRow = new StoredMessage { BodyUrl = uri };

                sched.AdvanceByMs(500);

                ServiceControl.Received(1).GetBody(uri);
            });
        }

        [Test]
        public void Should_load_body_content_when_body_tab_is_focused()
        {
            const string uri = "http://localhost:3333/api/somemessageid/body";

            var messageList = MessageListFunc();

            messageList.FocusedRow = new StoredMessage { BodyUrl = uri };

            messageList.Handle(new BodyTabSelectionChanged(true));

            ServiceControl.Received(1).GetBody(uri);
        }
    }
}