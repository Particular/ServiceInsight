namespace Particular.ServiceInsight.Desktop.Shell
{
    using Caliburn.Micro;
    using Events;

    public class StatusBarManager : PropertyChangedBase,
        IHandle<WorkStarted>,
        IHandle<WorkFinished>,
        IHandle<AsyncOperationFailed>
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

        public void Handle(AsyncOperationFailed @event)
        {
            StatusMessage = @event.Message;
            ErrorMessageVisible = true;
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

        public void Done()
        {
            if (!ErrorMessageVisible)
            {
                StatusMessage = DoneStatusMessage;
            }
        }
    }
}