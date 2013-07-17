using System.IO;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Core;
using NServiceBus.Profiler.Core.Licensing;
using NServiceBus.Profiler.Desktop.ScreenManager;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public class LicenseRegistrationViewModel : Screen, ILicenseRegistrationViewModel
    {
        private readonly ILicenseManager _licenseManager;
        private readonly IDialogManager _dialogManager;
        private readonly INetworkOperations _network;

        public const string LicensingPageUrl = "http://particular.net/licensing";

        public LicenseRegistrationViewModel(
            ILicenseManager licenseManager, 
            IDialogManager dialogManager,
            INetworkOperations network)
        {
            _licenseManager = licenseManager;
            _dialogManager = dialogManager;
            _network = network;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            License = _licenseManager.CurrentLicense;
            DisplayName = GetScreenTitle();
        }

        private string GetScreenTitle()
        {
            if (HasRemainingTrial) return string.Format("ServiceInsight - {0} day(s) left on your free trial", TrialDaysRemaining);
            if(HasFullLicense) return "ServiceInsight"; 
            
            return string.Format("ServiceInsight - Trial Expired");
        }

        public ProfilerLicense License { get; set; }

        public string LicenseType
        {
            get { return License == null ? ProfilerLicenseTypes.Trial : License.LicenseType; }
        }
        
        public string RegisteredTo
        {
            get { return HasTrialLicense ? ProfilerLicense.UnRegisteredUser : License.RegisteredTo; }
        }

        public int TrialDaysRemaining
        {
            get { return _licenseManager.GetRemainingTrialDays(); }
        }

        public bool HasTrialLicense
        {
            get { return LicenseType == ProfilerLicenseTypes.Trial; }
        }

        public bool HasFullLicense
        {
            get { return LicenseType == ProfilerLicenseTypes.Standard; }
        }

        public bool HasRemainingTrial
        {
            get { return HasTrialLicense && TrialDaysRemaining > 0; }
        }

        public bool AllowedToUse
        {
            get { return HasRemainingTrial || HasFullLicense; }
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

            if (dialog.Result.GetValueOrDefault(false))
            {
                var licenseContent = ReadAllTextWithoutLocking(dialog.FileName);
                _licenseManager.Initialize(licenseContent);
                License = _licenseManager.CurrentLicense;
            }

            if (License != null)
            {
                TryClose(true);
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