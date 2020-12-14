namespace ServiceInsight.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using Caliburn.Micro;
    using NSubstitute;
    using NUnit.Framework;
    using Explorer.EndpointExplorer;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.Settings;
    using MessageList;
    using MessageProperties;
    using Models;
    using Search;
    using ServiceControl;
    using Shouldly;

    [TestFixture]
    public class MessageListViewModelTests
    {
        IEventAggregator eventAggregator;
        IWorkNotifier workNotifier;
        IServiceControl serviceControl;
        SearchBarViewModel searchBar;
        Func<MessageListViewModel> messageListFunc;
        IClipboard clipboard;
        ISettingsProvider settingsProvider;
        ServiceControlClientRegistry clientRegistry;

        [SetUp]
        public void TestInitialize()
        {
            eventAggregator = Substitute.For<IEventAggregator>();
            workNotifier = Substitute.For<IWorkNotifier>();
            serviceControl = Substitute.For<IServiceControl>();
            searchBar = Substitute.For<SearchBarViewModel>();
            clipboard = Substitute.For<IClipboard>();
            settingsProvider = Substitute.For<ISettingsProvider>();
            clientRegistry = Substitute.For<ServiceControlClientRegistry>();

            clientRegistry.GetServiceControl(Arg.Any<string>()).Returns(serviceControl);
            
            messageListFunc = () => new MessageListViewModel(
                eventAggregator,
                workNotifier,
                searchBar,
                Substitute.For<GeneralHeaderViewModel>(),
                Substitute.For<MessageSelectionContext>(),
                clipboard,
                settingsProvider,
                clientRegistry);
        }

        [Test]
        public async Task Should_load_the_messages_from_the_endpoint()
        {
            var endpoint = new Endpoint { Host = "localhost", Name = "Service" };
            
            serviceControl.GetAuditMessages(Arg.Is(endpoint), 0, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>())
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

            var messageList = messageListFunc();

            var serviceControlNode = new ServiceControlExplorerItem("http://localhost:3333/api");
            var auditNode = new AuditEndpointExplorerItem(serviceControlNode, endpoint);
            await messageList.Handle(new SelectedExplorerItemChanged(auditNode));

            messageList.Rows.Count.ShouldBe(2);
            searchBar.IsVisible.ShouldBe(true);
        }

        [Test]
        public void All_MessageStatuses_Should_Translate_To_Image_Name([Values]MessageStatus status)
        {
            ImageSource TestResourceFinder(string name)
            {
                return new DrawingImage();
            }

            var message = new StoredMessage { Status = status };
            var icon = new MessageStatusIconInfo(message, TestResourceFinder);
            
            icon.Image.ShouldNotBe(null);
            icon.Description.ShouldNotBeNullOrEmpty();
            icon.Status.ShouldBe(status);
        }
    }
}