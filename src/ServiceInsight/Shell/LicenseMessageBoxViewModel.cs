namespace ServiceInsight.Shell
{
    using Caliburn.Micro;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.Licensing;
    using ServiceInsight.Framework.UI.ScreenManager;

    public class LicenseMessageBoxViewModel : Screen
    {
        private readonly AppLicenseManager licenseManager;
        private readonly NetworkOperations networkOperations;
        private readonly IWindowManagerEx windowManager;
        private readonly IAppCommands appCommands;

        public LicenseMessageBoxViewModel(
            AppLicenseManager licenseManager,
            NetworkOperations networkOperations,
            IWindowManagerEx windowManager,
            IAppCommands appCommands)
        {
            this.licenseManager = licenseManager;
            this.networkOperations = networkOperations;
            this.windowManager = windowManager;
            this.appCommands = appCommands;

            SetupLicenseOptions();
        }

        private void SetupLicenseOptions()
        {
            ResetLicenseOptions();

            if (licenseManager.HasTrialLicense)
            {
                ShowManageLicenseOption = true;
                ShowExtendTrialOption = true;
            }
            else
            {
                ShowManageLicenseCTAOption = true;
                ShowContactUsOption = true;
            }

            Message = LicenseStatusBar.SubscriptionLicenseText;
            ShowExitOption = true;
        }

        private void ResetLicenseOptions()
        {
            Result = null;
            ShowManageLicenseOption = false;
            ShowExtendTrialOption = false;
            ShowManageLicenseCTAOption = false;
            ShowExitOption = false;
            ShowContactUsOption = false;
        }

        public void Exit()
        {
            appCommands.ShutdownImmediately();
        }

        public void ExtendTrial()
        {
            networkOperations.OpenExtendTrial();
        }

        public void ManageLicenseCTA()
        {
            ManageLicense();
        }

        public void ManageLicense()
        {
            var result = windowManager.ShowDialog<ManageLicenseViewModel>();
            if (result.HasValue && result.Value)
            {
                if (!licenseManager.IsLicenseExpired())
                {
                    Result = true;
                    TryClose(Result);
                }
            }
        }

        public void ContactUs()
        {
            networkOperations.OpenContactUs();
        }

        public string Message { get; set; }

        public bool? Result { get; set; }

        public bool ShowManageLicenseCTAOption { get; set; }

        public bool ShowContactUsOption { get; set; }

        public bool ShowExtendTrialOption { get; set; }

        public bool ShowManageLicenseOption { get; set; }

        public bool ShowExitOption { get; set; }
    }
}