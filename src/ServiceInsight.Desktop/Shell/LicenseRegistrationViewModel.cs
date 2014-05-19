namespace Particular.ServiceInsight.Desktop.Shell
{
    using System.IO;
    using Caliburn.Micro;
    using Core;
    using Core.Licensing;
    using Core.UI.ScreenManager;
    using Licensing;

    public class LicenseRegistrationViewModel : Screen
    {
        AppLicenseManager licenseManager;
        IWindowManagerEx dialogManager;
        NetworkOperations network;

        public LicenseRegistrationViewModel(
            AppLicenseManager licenseManager,
            IWindowManagerEx dialogManager,
            NetworkOperations network)
        {
            this.licenseManager = licenseManager;
            this.dialogManager = dialogManager;
            this.network = network;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            DisplayName = GetScreenTitle();
        }

        string GetScreenTitle()
        {
            var expired = LicenseExpirationChecker.HasLicenseExpired(licenseManager.CurrentLicense);

            if (licenseManager.CurrentLicense.IsCommercialLicense)
            {
                if (expired)
                {
                    return "ServiceInsight - License Expired";
                }
                return "ServiceInsight";
            }
            if (HasRemainingTrial)
            {
                return string.Format("ServiceInsight - {0} day(s) left on your free trial", TrialDaysRemaining);
            }
            return string.Format("ServiceInsight - {0} Trial Expired", TrialTypeText);
        }

        public string TrialTypeText
        {
            get
            {
                return licenseManager.CurrentLicense.IsExtendedTrial ? "Extended" : "Initial";
            }
        }

        public string LicenseType
        {
            get { return licenseManager.CurrentLicense.LicenseType; }
        }

        public string RegisteredTo
        {
            get { return licenseManager.CurrentLicense.RegisteredTo; }
        }

        public int TrialDaysRemaining
        {
            get { return licenseManager.GetRemainingTrialDays(); }
        }

        public bool HasTrialLicense
        {
            get { return licenseManager.CurrentLicense.IsTrialLicense; }
        }

        public bool HasFullLicense
        {
            get { return licenseManager.CurrentLicense.IsCommercialLicense; }
        }

        public bool HasRemainingTrial
        {
            get { return HasTrialLicense && TrialDaysRemaining > 0; }
        }

        public bool AllowedToUse
        {
            get { return HasRemainingTrial || HasFullLicense; }
        }

        public bool CanExtendTrial
        {
            get
            {
                return HasTrialLicense && !licenseManager.CurrentLicense.IsExtendedTrial;
            }
        }

        public bool CanBuyNow
        {
            get
            {
                return CanContactSales;
            }
        }

        public bool CanContactSales
        {
            get
            {
                return HasTrialLicense && licenseManager.CurrentLicense.IsExtendedTrial;
            }
        }

        public bool MustExtendTrial
        {
            get
            {
                return HasTrialLicense && !HasRemainingTrial && !licenseManager.CurrentLicense.IsExtendedTrial;
            }
        }

        public bool MustPurchase
        {
            get
            {
                return HasTrialLicense && !HasRemainingTrial && licenseManager.CurrentLicense.IsExtendedTrial;
            }
        }

        public void OnLicenseChanged()
        {
            NotifyOfPropertyChange(() => LicenseType);
            NotifyOfPropertyChange(() => RegisteredTo);
            NotifyOfPropertyChange(() => TrialDaysRemaining);
        }

        public void LoadLicense()
        {
            var dialog = dialogManager.OpenFileDialog(new FileDialogModel
            {
                Filter = "License files (*.xml)|*.xml|All files (*.*)|*.*",
                FilterIndex = 1
            });

            var validLicense = false;

            if (dialog.Result.GetValueOrDefault(false))
            {
                var licenseContent = ReadAllTextWithoutLocking(dialog.FileName);

                validLicense = licenseManager.TryInstallLicense(licenseContent);
            }

            if (validLicense && !LicenseExpirationChecker.HasLicenseExpired(licenseManager.CurrentLicense))
            {
                TryClose(true);
                return;
            }
            //todo: Display error saying that the license was invalid
        }

        public void Close()
        {
            TryClose(AllowedToUse);
        }

        public void Purchase()
        {
            network.Browse("http://particular.net/licensing");
        }

        public void Extend()
        {
            network.Browse("http://particular.net/extend-your-trial-14");
        }

        public void ContactSales()
        {
            network.Browse("http://particular.net/extend-your-trial-45");
        }

        string ReadAllTextWithoutLocking(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var textReader = new StreamReader(fileStream))
            {
                return textReader.ReadToEnd();
            }
        }
    }
}