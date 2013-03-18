using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Desktop.Shell;

namespace NServiceBus.Profiler.Desktop.MessageList
{
    public interface IMessageListViewModel : IScreen, 
        IHandle<SelectedQueueChanged>, 
        IHandle<MessageRemovedFromQueue>,
        IHandle<AutoRefreshBeat>,
        IHandle<LoadAuditMessages>,
        IHandle<ErrorQueueSelected>,
        IHandle<WorkStarted>,
        IHandle<WorkFinished>,
        IWorkTracker
    {
        IObservableCollection<MessageInfo> Messages { get; }
        IObservableCollection<MessageInfo> SelectedMessages { get; }
        MessageInfo FocusedMessage { get; set; }
        Queue SelectedQueue { get; set; }
        void PurgeQueue();
        void DeleteSelectedMessages();
        void RefreshMessages();
        string GetCriticalTime(StoredMessage msg);
    }
}