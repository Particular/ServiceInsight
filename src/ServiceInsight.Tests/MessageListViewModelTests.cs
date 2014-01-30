using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.PresentationFramework.ApplicationModel;
using ExceptionHandler;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer;
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
    public class MessageListViewModelTests
    {
        private IEventAggregator EventAggregator;
        private IServiceControl ServiceControl;
        private ISearchBarViewModel SearchBar;
        private MessageListViewModel MessageList;
        private IMessageListView View;

        [SetUp]
        public void TestInitialize()
        {
            EventAggregator = Substitute.For<IEventAggregator>();
            ServiceControl = Substitute.For<IServiceControl>();
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
                             .Returns(x => Task.FromResult(new PagedResult<StoredMessage>
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
        public void Should_load_body_content_when_body_tab_is_already_selected()
        {
            const string uri = "http://localhost:3333/api/somemessageid/body";

            MessageList.FocusedRow = null;
            MessageList.Handle(new BodyTabSelectionChanged(true));
            
            AsyncHelper.Run(() =>
            {
                MessageList.FocusedRow = new StoredMessage {BodyUrl = uri};
            });

            ServiceControl.Received(1).GetBody(uri).IgnoreAwait();
        }

        [Test]
        public void Should_load_body_content_when_body_tab_is_focused()
        {
            const string uri = "http://localhost:3333/api/somemessageid/body";
            MessageList.FocusedRow = new StoredMessage { BodyUrl = uri };

            AsyncHelper.Run(() => MessageList.Handle(new BodyTabSelectionChanged(true)));

            ServiceControl.Received(1).GetBody(uri).IgnoreAwait();
        }
    }
}