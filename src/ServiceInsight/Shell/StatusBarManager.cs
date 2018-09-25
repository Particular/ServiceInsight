namespace ServiceInsight.Shell
{
    using Caliburn.Micro;
    using ServiceInsight.Framework.Events;

    public class StatusBarManager : Screen,
        IHandle<WorkStarted>,
        IHandle<WorkFinished>,
        IHandle<AsyncOperationFailed>
    {
        public const string DoneStatusMessage = "Done";

        public StatusBarManager(LicenseStatusBar licenseStatusBar)
        {
            LicenseStatus = licenseStatusBar;
        }

        public string StatusMessage { get; private set; }

        public bool ErrorMessageVisible { get; private set; }

        public LicenseStatusBar LicenseStatus { get; }

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