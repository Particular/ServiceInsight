namespace Particular.ServiceInsight.Desktop.Shell
{
    using System.IO;
    using Caliburn.PresentationFramework.Screens;
    using Core;
    using Core.Licensing;
    using Core.UI.ScreenManager;
    using Licensing;

    public class LicenseRegistrationViewModel : Screen, ILicenseRegistrationViewModel
    {
        private readonly AppLicenseManager licenseManager;
        private readonly IDialogManager _dialogManager;
        private readonly INetworkOperations _network;

        public const string LicensingPageUrl = "http://particular.net/licensing";
        public const string LicenseExtensionPageUrl = "http://particular.net/extend-your-trial-14";
        public const string CustomLicenseExtensionPageUrl = "http://particular.net/extend-your-trial-45";

        public LicenseRegistrationViewModel(
            AppLicenseManager licenseManager,
            IDialogManager dialogManager,
            INetworkOperations network)
        {
            this.licenseManager = licenseManager;
            _dialogManager = dialogManager;
            _network = network;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            DisplayName = GetScreenTitle();
        }

        private string GetScreenTitle()
        {
            var expired = LicenseExpirationChecker.HasLicenseExpired(licenseManager.CurrentLicense);


            if (licenseManager.CurrentLicense.IsCommercialLicense)
            {
                if (expired)
                {
                    return string.Format("ServiceInsight - License Expired");
                }
                else
                {
                    return string.Format("ServiceInsight");
                }

            }
            else
            {
                if (HasRemainingTrial)
                {
                    return string.Format("ServiceInsight - {0} day(s) left on your free trial", TrialDaysRemaining);
                }
                else
                {
                    return string.Format("ServiceInsight - {0} Trial Expired", TrialTypeText);
                }
            }
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
            var dialog = _dialogManager.OpenFileDialog(new FileDialogModel
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
            }
            else
            {
                //todo: Display error saying that the license was invalid
            }
        }

        public void Close()
        {
            TryClose(AllowedToUse);
        }

        public void Purchase()
        {
            _network.Browse(LicensingPageUrl);
        }

        public void Extend()
        {
            _network.Browse(LicenseExtensionPageUrl);
        }

        public void ContactSales()
        {
            _network.Browse(CustomLicenseExtensionPageUrl);
        }


        private string ReadAllTextWithoutLocking(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var textReader = new StreamReader(fileStream))
            {
                return textReader.ReadToEnd();
            }
        }
    }

    public interface ILicenseRegistrationViewModel : IScreen
    {
        string LicenseType { get; }
        string RegisteredTo { get; }
        int TrialDaysRemaining { get; }
        bool HasTrialLicense { get; }
        void LoadLicense();
        void Close();
    }
}