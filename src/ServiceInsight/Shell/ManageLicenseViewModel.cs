namespace ServiceInsight.Shell
{
    using System;
    using System.IO;
    using System.Windows.Input;
    using Caliburn.Micro;
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

            ContactUsCommand = Command.Create(ContactUs);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            ValidationResult = null;
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

            LicenseType = currentLicense.LicenseType;

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
        }

        private void ContactUs()
        {
            network.OpenContactUs();
        }

        public void ImportLicense()
        {
            var dialog = windowManager.OpenFileDialog(new FileDialogModel
            {
                Filter = "License files (*.xml)|*.xml|All files (*.*)|*.*",
                FilterIndex = 1
            });

            var validLicense = false;

            if (dialog.Result.HasValue && dialog.Result.Value)
            {
                var licenseContent = ReadAllTextWithoutLocking(dialog.FileName);
                validLicense = licenseManager.TryInstallLicense(licenseContent);

                if (licenseManager.IsLicenseExpired())
                {
                    validLicense = false;
                }
            }

            if (validLicense)
            {
                ShowLicenseStatus();
                eventAggregator.PublishOnUIThread(new LicenseUpdated());
                ValidationResult = dialog.Result;
            }
        }

        public ICommand ContactUsCommand { get; }

        public string LicenseType { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public DateTime? UpgradeProtectionExpirationDate { get; set; }

        public bool? ValidationResult { get; set; }
    }
}