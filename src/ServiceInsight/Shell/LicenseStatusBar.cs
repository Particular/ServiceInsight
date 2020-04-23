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
        public const string UpgradeProtectionLicenseText = "Please extend your upgrade protection so that we can continue to provide you with support and new versions of the Particular Service Platform.";
        public const string SubscriptionLicenseText = "Please extend your license to continue using the Particular Service Platform.";

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
            get { return HasLicenseMessage && forceShowPopup; }
            set => forceShowPopup = value;
        }

        public string LicenseStatusMessage { get; private set; }

        public ICommand ManageLicense { get; set; }

        public ICommand ContactUs { get; set; }

        public bool AppStartCheck { get; set; }

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
            }
            else if (remainingDays == 0)
            {
                LicenseStatusMessage = UpgradeProtectionExpiresTodayMessage;
            }
            else if (remainingDays <= 10)
            {
                LicenseStatusMessage = string.Format(UpgradeProtectionExpiringMessage, remainingDays);
            }

            LicensePopupText = UpgradeProtectionLicenseText;
            ShowLicenseWarn = true;
            OpenLicensePopup = AppStartCheck; //Show the license popup only at app start
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
                ShowLicenseWarn = true;
                LicenseStatusMessage = LicenseExpiresTodayMessage;
            }
            else if (remainingDays < 0)
            {
                ShowLicenseError = true;
                LicenseStatusMessage = LicenseExpiredMessage;
            }
            else if (remainingDays <= 10)
            {
                ShowLicenseWarn = true;
                LicenseStatusMessage = string.Format(LicenseExpiringMessage, remainingDays);
            }

            LicensePopupText = SubscriptionLicenseText;
            OpenLicensePopup = AppStartCheck;
        }

        public void SetTrialRemainingDays(int remainingDays)
        {
            ResetStatusBar();

            if (remainingDays > 10)
            {
                return;
            }

            if (remainingDays <= 0)
            {
                ShowLicenseError = true;
                LicenseStatusMessage = LicenseExpiredMessage;
                LicensePopupText = SubscriptionLicenseText;
            }
            else if (remainingDays <= 10)
            {
                ShowLicenseWarn = true;
                LicenseStatusMessage = string.Format(LicenseExpiringMessage, remainingDays);
                LicensePopupText = SubscriptionLicenseText;
            }

            OpenLicensePopup = AppStartCheck;
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