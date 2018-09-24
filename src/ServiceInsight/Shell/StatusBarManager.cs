namespace ServiceInsight.Shell
{
    using System.Windows.Input;
    using Caliburn.Micro;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.UI.ScreenManager;

    public class LicenseStatusBar : Screen
    {
        public const string LicenseExpiringMessage = "License expiring in {0} day(s)";
        public const string LicenseExpiredMessage = "License expired";
        public const string UpgradeProtectionExpiringMessage = "Upgrade protection expiring in {0} day(s)";
        public const string UpgradeProtectionExpiredMessage = "Upgrade protection expired";
        public const string TrialExpiringMessage = "Trial expiring in {0} day(s)";
        public const string TrialExpiredMessage = "Trial expired";
        public const string UpgradeProtectionExpiringText = "Once upgrade protection expires you'll no longer have access to new product versions.";
        public const string UpgradeProtectionExpiredText = "You'll no longer have access to new product versions. Please import a new license or contact us.";
        public const string LicenseExpiringText = "Once the license expires you'll no longer be able to continue using the application.";
        public const string LicenseExpiredText = "You are no longer able to continue using the application. Please import a new license or contact us.";

        private readonly IWindowManagerEx windowManager;
        private readonly NetworkOperations network;
        private bool forceShowPopup;

        public LicenseStatusBar(IWindowManagerEx windowManager, NetworkOperations network)
        {
            this.network = network;
            this.windowManager = windowManager;

            ContactUs = Command.Create(OnContactUsClicked);
            ManageLicense = Command.Create(OnManageLicense);
        }

        public string Registration { get; private set; }

        public string LicensePopupText { get; private set; }

        public bool ShowLicenseError { get; private set; }

        public bool ShowLicenseWarn { get; private set; }

        public bool OpenLicensePopup
        {
            get => (ShowLicenseWarn || ShowLicenseError) && forceShowPopup;
            set => forceShowPopup = value;
        }

        public string LicenseStatusMessage { get; private set; }

        public ICommand ManageLicense { get; set; }

        public ICommand ContactUs { get; set; }

        public void SetRegistrationInfo(string message, params object[] args)
        {
            Registration = string.Format(message, args);
        }

        public void SetLicenseUpgradeProtectionDays(int remainingDays)
        {
            if (remainingDays > 10)
            {
                return;
            }

            if (remainingDays == 0)
            {
                LicenseStatusMessage = UpgradeProtectionExpiredMessage;
                LicensePopupText = UpgradeProtectionExpiredText;
            }

            if (remainingDays <= 10)
            {
                LicenseStatusMessage = string.Format(UpgradeProtectionExpiringMessage, remainingDays);
                LicensePopupText = UpgradeProtectionExpiringText;
            }

            ShowLicenseWarn = true;
            OpenLicensePopup = true;
        }

        public void SetLicenseRemainingDays(int remainingDays)
        {
            if (remainingDays > 10)
            {
                return;
            }

            if (remainingDays == 0)
            {
                ShowLicenseError = true;
                LicenseStatusMessage = LicenseExpiredMessage;
                LicensePopupText = LicenseExpiredText;
            }
            else if (remainingDays <= 10)
            {
                ShowLicenseWarn = true;
                LicenseStatusMessage = string.Format(LicenseExpiringMessage, remainingDays);
                LicensePopupText = LicenseExpiringText;
            }

            OpenLicensePopup = true;
        }

        public void SetTrialRemainingDays(int remainingDays)
        {
            if (remainingDays > 10)
            {
                return;
            }

            if (remainingDays == 0)
            {
                ShowLicenseError = true;
                LicenseStatusMessage = LicenseExpiredMessage;
                LicensePopupText = LicenseExpiredText;
            }
            else if (remainingDays <= 10)
            {
                ShowLicenseWarn = true;
                LicenseStatusMessage = string.Format(LicenseExpiringMessage, remainingDays);
                LicensePopupText = LicenseExpiringText;
            }

            OpenLicensePopup = true;
        }

        private void OnManageLicense()
        {
            windowManager.ShowDialog<LicenseRegistrationViewModel>();
        }

        private void OnContactUsClicked()
        {
            network.Browse("http://particular.net/contactus");
        }
    }

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