namespace ServiceInsight.Shell
{
    using Caliburn.Micro;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.Licensing;
    using ServiceInsight.Framework.UI.ScreenManager;

    public class LicenseMessageBoxViewModel : Screen
    {
        readonly AppLicenseManager licenseManager;
        readonly NetworkOperations networkOperations;
        readonly IServiceInsightWindowManager windowManager;
        readonly IAppCommands appCommands;

        public LicenseMessageBoxViewModel(
            AppLicenseManager licenseManager,
            NetworkOperations networkOperations,
            IServiceInsightWindowManager windowManager,
            IAppCommands appCommands)
        {
            this.licenseManager = licenseManager;
            this.networkOperations = networkOperations;
            this.windowManager = windowManager;
            this.appCommands = appCommands;

            SetupLicenseOptions();
        }

        void SetupLicenseOptions()
        {
            ResetLicenseOptions();

            if (licenseManager.HasNonProductionLicense)
            {
                ShowManageLicenseOption = true;
                ShowExtendLicenseOption = true;
            }
            else
            {
                ShowManageLicenseCTAOption = true;
                ShowContactUsOption = true;
            }

            Message = LicenseStatusBar.SubscriptionLicenseText;
            ShowExitOption = true;
        }

        void ResetLicenseOptions()
        {
            Result = null;
            ShowManageLicenseOption = false;
            ShowExtendLicenseOption = false;
            ShowManageLicenseCTAOption = false;
            ShowExitOption = false;
            ShowContactUsOption = false;
        }

        public void Exit()
        {
            appCommands.ShutdownImmediately();
        }

        public void ExtendLicense()
        {
            networkOperations.OpenExtendLicense(licenseManager.HasEvaluationLicense);
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

        public bool ShowExtendLicenseOption { get; set; }

        public bool ShowManageLicenseOption { get; set; }

        public bool ShowExitOption { get; set; }
    }
}