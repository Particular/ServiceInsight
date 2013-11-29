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
        public const string DoneStatusMessage = "Done";

        public string StatusMessage { get; private set; }

        public string Registration { get; private set; }

        public bool ErrorMessageVisible { get; private set; }

        public void Handle(WorkStarted @event)
        {
            SetSuccessStatusMessage(@event.Message);
        }

        public void Handle(WorkFinished @event)
        {
            if (!ErrorMessageVisible)
            {
                SetSuccessStatusMessage(@event.Message);
            }
        }

        public void SetRegistrationInfo(string message, params object[] args)
        {
            Registration = string.Format(message, args);
        }

        public void SetSuccessStatusMessage(string message, params object[] args)
        {
            StatusMessage = string.Format(message, args);
            ErrorMessageVisible = false;
        }

        public void SetFailStatusMessage(string message, params object[] args)
        {
            StatusMessage = string.Format(message, args);
            ErrorMessageVisible = true;
        }

        public void Done()
        {
            if (!ErrorMessageVisible)
            {
                StatusMessage = DoneStatusMessage;
            }
        }
    }
}