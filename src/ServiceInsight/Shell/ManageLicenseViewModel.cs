namespace ServiceInsight.Shell
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Input;
    using Caliburn.Micro;
    using ServiceInsight.ExtensionMethods;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.Licensing;
    using ServiceInsight.Framework.UI.ScreenManager;

    public class ManageLicenseViewModel : Screen
    {
        private readonly AppLicenseManager licenseManager;
        private readonly NetworkOperations network;
        private readonly IWindowManagerEx windowManager;
        private readonly IEventAggregator eventAggregator;
        private static readonly IDictionary<LicenseInstallationResult, string> InstallationResultMessage;

        static ManageLicenseViewModel()
        {
            InstallationResultMessage = new Dictionary<LicenseInstallationResult, string>
            {
                [LicenseInstallationResult.Succeeded] = "License imported successfully",
                [LicenseInstallationResult.Expired] = "License failed to import because the license has expired",
                [LicenseInstallationResult.Failed] = "License failed to import"
            };
        }

        public ManageLicenseViewModel(
            AppLicenseManager licenseManager,
            NetworkOperations network,
            IWindowManagerEx windowManager,
            IEventAggregator eventAggregator)
        {
            this.licenseManager = licenseManager;
            this.network = network;
            this.windowManager = windowManager;
            this.eventAggregator = eventAggregator;

            DisplayName = "License Information";
            ContactUsCommand = Command.Create(ContactUs);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            ImportMessage = null;
            ValidationResult = null;
            ImportWasSuccessful = null;
            ShowLicenseStatus();
        }

        string ReadAllTextWithoutLocking(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var textReader = new StreamReader(fileStream))
            {
                return textReader.ReadToEnd();
            }
        }

        private void ShowLicenseStatus()
        {
            var currentLicense = licenseManager.CurrentLicense;
            if (currentLicense == null)
            {
                return;
            }

            LicenseType = currentLicense.IsTrialLicense ? currentLicense.LicenseType : string.Format($"{currentLicense.LicenseType}, {currentLicense.Edition}");

            if (currentLicense.IsCommercialLicense)
            {
                UpgradeProtectionExpirationDate = currentLicense.UpgradeProtectionExpiration;
                ExpirationDate = currentLicense.ExpirationDate;
            }
            else
            {
                UpgradeProtectionExpirationDate = null;
                ExpirationDate = currentLicense.ExpirationDate;
            }

            if (currentLicense.IsCommercialLicense)
            {
                var expirationDaysLeft = licenseManager.GetExpirationRemainingDays();
                ExpirationDateStatus = licenseManager.GetExpirationStatus();
                ExpirationRemainingDays = expirationDaysLeft.HasValue && expirationDaysLeft.Value > 0 ? expirationDaysLeft.Value : 0;

                var upgradeDaysLeft = licenseManager.GetUpgradeProtectionRemainingDays();
                UpgradeProtectionDateStatus = licenseManager.GetUpgradeProtectionStatus();
                UpgradeProtectionRemainingDays = upgradeDaysLeft.HasValue && upgradeDaysLeft.Value > 0 ? upgradeDaysLeft.Value : 0;
            }
            else
            {
                ExpirationRemainingDays = licenseManager.GetRemainingNonProductionDays();
                ExpirationDateStatus = licenseManager.GetNonProductionExpirationStatus();
            }
        }

        private void ContactUs()
        {
            network.OpenContactUs();
        }

        public void ExtendLicense()
        {
            network.OpenExtendLicense(licenseManager.HasEvaluationLicense);
        }

        public void ImportLicense()
        {
            var dialog = windowManager.OpenFileDialog(new FileDialogModel
            {
                Filter = "License files (*.xml)|*.xml|All files (*.*)|*.*",
                FilterIndex = 1
            });

            var result = LicenseInstallationResult.Failed;

            if (dialog.Result.HasValue && dialog.Result.Value)
            {
                var licenseContent = ReadAllTextWithoutLocking(dialog.FileName);
                result = licenseManager.TryInstallLicense(licenseContent);

                ImportMessage = InstallationResultMessage[result];
                ImportWasSuccessful = result == LicenseInstallationResult.Succeeded;
            }

            if (result == LicenseInstallationResult.Succeeded)
            {
                ShowLicenseStatus();
                NotifyOfPropertyChange(nameof(CanExtendLicense));
                eventAggregator.PublishOnUIThread(new LicenseUpdated());
                ValidationResult = dialog.Result;
            }
        }

        public ICommand ContactUsCommand { get; }

        public string LicenseType { get; set; }

        public bool CanExtendLicense => licenseManager.HasNonProductionLicense;

        public DateTime? ExpirationDate { get; set; }

        public int? ExpirationRemainingDays { get; set; }

        public int? UpgradeProtectionRemainingDays { get; set; }

        public bool ShowExpirationMessage => ShowExpirationWarning || ShowExpirationError;

        public bool ShowExpirationError => ExpirationDateStatus.In(DateExpirationStatus.Expired);

        public bool ShowExpirationWarning => ExpirationDateStatus.In(DateExpirationStatus.Expiring);

        public bool ShowUpgradeProtectionWarning => UpgradeProtectionDateStatus.In(DateExpirationStatus.Expired, DateExpirationStatus.Expiring);

        public DateExpirationStatus ExpirationDateStatus { get; set; }

        public DateExpirationStatus UpgradeProtectionDateStatus { get; set; }

        public DateTime? UpgradeProtectionExpirationDate { get; set; }

        public bool? ValidationResult { get; set; }

        public bool? ImportWasSuccessful { get; set; }

        public string ImportMessage { get; set; }
    }
}