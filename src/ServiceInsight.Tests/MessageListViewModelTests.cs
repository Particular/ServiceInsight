﻿namespace ServiceInsight.Tests
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;
    using NSubstitute;
    using NUnit.Framework;
    using ServiceInsight.Explorer.EndpointExplorer;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.Settings;
    using ServiceInsight.MessageList;
    using ServiceInsight.MessageProperties;
    using ServiceInsight.Models;
    using ServiceInsight.Search;
    using ServiceInsight.ServiceControl;
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

        [SetUp]
        public void TestInitialize()
        {
            eventAggregator = Substitute.For<IEventAggregator>();
            workNotifier = Substitute.For<IWorkNotifier>();
            serviceControl = Substitute.For<IServiceControl>();
            searchBar = Substitute.For<SearchBarViewModel>();
            clipboard = Substitute.For<IClipboard>();
            settingsProvider = Substitute.For<ISettingsProvider>();
            messageListFunc = () => new MessageListViewModel(
                eventAggregator,
                workNotifier,
                serviceControl,
                searchBar,
                Substitute.For<GeneralHeaderViewModel>(),
                Substitute.For<MessageSelectionContext>(),
                clipboard,
                settingsProvider);
        }

        [Test]
        public void Should_load_the_messages_from_the_endpoint()
        {
            var endpoint = new Endpoint { Host = "localhost", Name = "Service" };
            serviceControl.GetAuditMessages(Arg.Is(endpoint), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>())
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

            messageList.Handle(new SelectedExplorerItemChanged(new AuditEndpointExplorerItem(endpoint)));

            messageList.Rows.Count.ShouldBe(2);
            searchBar.IsVisible.ShouldBe(true);
        }
    }
}