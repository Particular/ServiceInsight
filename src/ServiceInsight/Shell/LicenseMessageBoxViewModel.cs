﻿namespace ServiceInsight.Shell
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
                ShowExtendTrialOption = true;
                ShowExitOption = true;
            }
            else
            {
                ShowContactUsOption = true;
            }

            ShowManageLicenseOption = true;
            ShowExitOption = true;
        }

        private void ResetLicenseOptions()
        {
            Result = null;
            ShowExtendTrialOption = false;
            ShowManageLicenseOption = false;
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

        public void ManageLicense()
        {
            var result = windowManager.ShowDialog<ManageLicenseViewModel>();
            if (result.HasValue && result.Value)
            {
                if (!licenseManager.IsLicenseExpired())
                {
                    Result = result;
                    TryClose(Result);
                }
            }
        }

        public void ContactUs()
        {
            networkOperations.OpenContactUs();
        }

        public bool? Result { get; set; }

        public bool ShowManageLicenseOption { get; set; }

        public bool ShowContactUsOption { get; set; }

        public bool ShowExtendTrialOption { get; set; }

        public bool ShowExitOption { get; set; }
    }
}