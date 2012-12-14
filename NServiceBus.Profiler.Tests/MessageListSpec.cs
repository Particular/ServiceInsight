using System.Collections.Generic;
using Caliburn.PresentationFramework.ApplicationModel;
using Machine.Specifications;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core;
using NServiceBus.Profiler.Desktop.MessageList;
using NServiceBus.Profiler.Desktop.ScreenManager;
using NServiceBus.Profiler.Tests.Helpers;
using NSubstitute;

namespace NServiceBus.Profiler.Tests
{
    [Subject("message list")]
    public class with_the_message_list
    {
        protected static IMessageListViewModel MessageList;
        protected static IQueueManagerAsync QueueManager;
        protected static IWindowManagerEx WindowManager;
        protected static IEventAggregator EventAggregator;
        protected static Dictionary<Queue, List<MessageInfo>> MessageStore;
        
        Establish context = () =>
        {
            EventAggregator = Substitute.For<IEventAggregator>();
            MessageStore = new Dictionary<Queue, List<MessageInfo>>();
            QueueManager = new FakeQueueManager(MessageStore);
            WindowManager = Substitute.For<IWindowManagerEx>();
            MessageList = new MessageListViewModel(EventAggregator, WindowManager, QueueManager);
        };
    }

    public class loads_the_messages_from_the_queue : with_the_message_list
    {
        protected static Queue SelectedQueue;

        Establish context = () =>
        {
            SelectedQueue = new Queue("testqueue");
            MessageStore.Add(SelectedQueue, new List<MessageInfo>(new[] { new MessageInfo(), new MessageInfo() }));
        };

        Because of = () => MessageList.SelectedQueue = SelectedQueue; //Should trigger refresh

        It should_start_signaling_work_is_started = () => EventAggregator.Received(1).Publish(Arg.Any<WorkStartedEvent>());
        It should_load_the_messages_asynchronously = () => MessageList.Messages.Count.ShouldEqual(2);
    }
}