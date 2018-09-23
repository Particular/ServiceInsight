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
        public const string LicenseExpiringMessage = "License expiring in {0} day(s)";
        public const string LicenseExpiredMessage = "License expired";
        public const string UpgradeProtectionExpiringMessage = "Upgrade protection expiring in {0} day(s)";
        public const string UpgradeProtectionExpiredMessage = "Upgrade protection expired";
        public const string TrialExpiringMessage = "Trial expiring in {0} day(s)";
        public const string TrialExpiredMessage = "Trial expired";

        public string StatusMessage { get; private set; }

        public string Registration { get; private set; }

        public bool ErrorMessageVisible { get; private set; }

        public bool ShowLicenseError { get; private set; }

        public bool ShowLicenseWarn { get; private set; }

        public bool ShowLicensePopup => ShowLicenseWarn || ShowLicenseError;

        public string LicenseStatusMessage { get; private set; }

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

        public void SetLicenseUpgradeProtectionDays(int remainingDays)
        {
            if (remainingDays > 10)
            {
                return;
            }

            ShowLicenseWarn = true;

            if (remainingDays == 0)
            {
                LicenseStatusMessage = UpgradeProtectionExpiredMessage;
            }

            if (remainingDays <= 10)
            {
                LicenseStatusMessage = string.Format(UpgradeProtectionExpiringMessage, remainingDays);
            }
        }

        public void SetLicenseRemainingDays(int remainingDays)
        {
            if (remainingDays == 0)
            {
                ShowLicenseError = true;
                LicenseStatusMessage = LicenseExpiredMessage;
            }
            else if (remainingDays <= 10)
            {
                ShowLicenseWarn = true;
                LicenseStatusMessage = string.Format(LicenseExpiringMessage, remainingDays);
            }
        }

        public void SetTrialRemainingDays(int remainingDays)
        {
            if (remainingDays == 0)
            {
                ShowLicenseError = true;
                LicenseStatusMessage = LicenseExpiredMessage;
            }
            else if (remainingDays <= 10)
            {
                ShowLicenseWarn = true;
                LicenseStatusMessage = string.Format(LicenseExpiringMessage, remainingDays);
            }
        }
    }
}