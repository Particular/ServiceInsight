using System.IO;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Core.Licensing;
using NServiceBus.Profiler.Desktop.ScreenManager;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public class LicenseRegistrationViewModel : Screen, ILicenseRegistrationViewModel
    {
        private readonly ILicenseManager _licenseManager;
        private readonly IDialogManager _dialogManager;

        public LicenseRegistrationViewModel(
            ILicenseManager licenseManager, 
            IDialogManager dialogManager)
        {
            _licenseManager = licenseManager;
            _dialogManager = dialogManager;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            License = _licenseManager.CurrentLicense;
            DisplayName = "Registration";
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
            TryClose(!HasTrialLicense || TrialDaysRemaining > 0);
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