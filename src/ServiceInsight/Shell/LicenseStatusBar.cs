namespace ServiceInsight.Shell
{
    using System.Windows.Input;
    using Caliburn.Micro;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.UI.ScreenManager;

    public class LicenseStatusBar : Screen
    {
        public const string LicenseExpiringMessage = "License expiring in {0} day(s)";
        public const string LicenseExpiredMessage = "License expired";
        public const string LicenseExpiresTodayMessage = "License will expire today";
        public const string UpgradeProtectionExpiringMessage = "Upgrade protection expiring in {0} day(s)";
        public const string UpgradeProtectionExpiredMessage = "Upgrade protection expired";
        public const string UpgradeProtectionExpiresTodayMessage = "Upgrade protection expires today";
        public const string TrialExpiringMessage = "Trial expiring in {0} day(s)";
        public const string TrialExpiredMessage = "Trial expired";
        public const string UpgradeProtectionExpiringText = "Once upgrade protection expires you'll no longer have access to new product versions.";
        public const string UpgradeProtectionExpiredText = "You'll no longer have access to new product versions. Please import a new license or contact us.";
        public const string LicenseExpiringText = "Once the license expires you'll no longer be able to continue using the application.";
        public const string LicenseExpiredText = "You are no longer able to continue using the application. Please import a new license or contact us.";
        public const string LicenseExpiresTodayText = "You will no longer be able to continue using the application. Please import a new license or contact us.";

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

        public bool HasLicenseMessage => ShowLicenseError || ShowLicenseWarn;

        public bool OpenLicensePopup
        {
            get => HasLicenseMessage && forceShowPopup;
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
            ResetStatusBar();

            if (remainingDays > 10)
            {
                return;
            }

            if (remainingDays < 0)
            {
                LicenseStatusMessage = UpgradeProtectionExpiredMessage;
                LicensePopupText = UpgradeProtectionExpiredText;
            }
            else if (remainingDays == 0)
            {
                LicenseStatusMessage = UpgradeProtectionExpiresTodayMessage;
                LicensePopupText = UpgradeProtectionExpiredText;
            }
            else if (remainingDays <= 10)
            {
                LicenseStatusMessage = string.Format(UpgradeProtectionExpiringMessage, remainingDays);
                LicensePopupText = UpgradeProtectionExpiringText;
            }

            ShowLicenseWarn = true;
            OpenLicensePopup = true;
        }

        public void SetLicenseRemainingDays(int remainingDays)
        {
            ResetStatusBar();

            if (remainingDays > 10)
            {
                return;
            }

            if (remainingDays == 0)
            {
                ShowLicenseError = true;
                LicenseStatusMessage = LicenseExpiresTodayMessage;
                LicensePopupText = LicenseExpiresTodayText;
            }
            else if (remainingDays < 0)
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
            ResetStatusBar();

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

        private void ResetStatusBar()
        {
            OpenLicensePopup = false;
            ShowLicenseWarn = false;
            ShowLicenseError = false;
            LicenseStatusMessage = string.Empty;
            LicensePopupText = string.Empty;
        }

        private void OnManageLicense()
        {
            windowManager.ShowDialog<ManageLicenseViewModel>();
        }

        private void OnContactUsClicked()
        {
            network.OpenContactUs();
        }
    }
}