namespace ServiceInsight.Shell
{
    using System;
    using Caliburn.Micro;
    using Framework;
    using ServiceInsight.Framework.Events;

    public class StatusBarManager : PropertyChangedBase
    {
        public const string DoneStatusMessage = "Done";

        public StatusBarManager(IRxEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<WorkStarted>().Subscribe(Handle);
            eventAggregator.GetEvent<WorkFinished>().Subscribe(Handle);
            eventAggregator.GetEvent<AsyncOperationFailed>().Subscribe(Handle);
        }

        public string StatusMessage { get; private set; }

        public string Registration { get; private set; }

        public bool ErrorMessageVisible { get; private set; }

        void Handle(WorkStarted @event)
        {
            SetSuccessStatusMessage(@event.Message);
        }

        void Handle(WorkFinished @event)
        {
            if (!ErrorMessageVisible)
            {
                SetSuccessStatusMessage(@event.Message);
            }
        }

        void Handle(AsyncOperationFailed @event)
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