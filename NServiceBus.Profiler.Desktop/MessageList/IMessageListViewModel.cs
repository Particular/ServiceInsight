using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Desktop.MessageList
{
    public interface IMessageListViewModel : IScreen, 
        IHandle<SelectedQueueChangedEvent>, 
        IHandle<MessageRemovedFromQueueEvent>,
        IHandle<AutoRefreshBeatEvent>
    {
        IObservableCollection<MessageInfo> Messages { get; }
        IObservableCollection<MessageInfo> SelectedMessages { get; }
        MessageInfo FocusedMessage { get; set; }
        Queue SelectedQueue { get; set; }
        void PurgeQueue();
        void DeleteSelectedMessages();
        void RefreshMessages();
    }
}