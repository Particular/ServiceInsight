using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using NServiceBus.Profiler.Common.Events;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public class StatusBarManager : PropertyChangedBase, 
        IStatusBarManager, 
        IHandle<WorkStarted>,
        IHandle<WorkFinished>
    {
        public virtual string Status { get; set; }

        public virtual void Handle(WorkStarted @event)
        {
            Status = @event.Message;
        }

        public virtual void Handle(WorkFinished @event)
        {
            Status = @event.Message;
        }
    }
}