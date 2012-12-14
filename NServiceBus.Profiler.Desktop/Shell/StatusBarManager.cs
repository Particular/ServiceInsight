using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using NServiceBus.Profiler.Common.Events;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public class StatusBarManager : PropertyChangedBase, 
        IStatusBarManager, 
        IHandle<WorkStartedEvent>,
        IHandle<WorkFinishedEvent>
    {
        public virtual string Status { get; set; }

        public virtual void Handle(WorkStartedEvent @event)
        {
            Status = @event.Message;
        }

        public virtual void Handle(WorkFinishedEvent @event)
        {
            Status = @event.Message;
        }
    }
}