using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using NServiceBus.Profiler.Desktop.Events;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public class StatusBarManager : PropertyChangedBase, 
        IStatusBarManager, 
        IHandle<WorkStarted>,
        IHandle<WorkFinished>
    {
        public virtual string StatusMessage { get; set; }

        public virtual string Registration { get; set; }

        public virtual void Handle(WorkStarted @event)
        {
            StatusMessage = @event.Message;
        }

        public virtual void Handle(WorkFinished @event)
        {
            StatusMessage = @event.Message;
        }
    }
}