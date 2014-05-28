namespace Particular.ServiceInsight.Desktop.Shell
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Core;
    using ExtensionMethods;
    using Framework.Rx;
    using ServiceControl;

    public class AboutViewModel : INotifyPropertyChanged, IActivate, IHaveDisplayName
    {
        const string DetectingServiceControlVersion = "(Detecting...)";
        const string NotConnectedToServiceControl = "(Not Connected)";

        DefaultServiceControl serviceControl;

        public event EventHandler<ActivationEventArgs> Activated = delegate { };

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public bool IsSplash { get; private set; }

        public bool HasFullLicense { get { return License != null && !License.HasTrialLicense; } }

        public LicenseRegistrationViewModel License { get; private set; }

        public string AppVersion { get; private set; }

        public string ServiceControlVersion { get; private set; }

        public string CopyrightText { get; private set; }

        public string DisplayName { get; set; }

        public string CommitHash { get; private set; }

        public bool IsActive { get; private set; }

        public ICommand NavigateToSiteCommand { get; private set; }

        public AboutViewModel(
            NetworkOperations networkOperations,
            DefaultServiceControl serviceControl,
            LicenseRegistrationViewModel licenseInfo)
        {
            this.serviceControl = serviceControl;

            License = licenseInfo;
            IsSplash = false;
            DisplayName = "About";

            NavigateToSiteCommand = this.CreateCommand(() => networkOperations.Browse("http://www.particular.net"));
        }

        AboutViewModel()
        {
            IsSplash = true;
        }

        public static AboutViewModel AsSplashScreenModel()
        {
            var vm = new AboutViewModel();
            vm.Activate();
            return vm;
        }

        public async void Activate()
        {
            await OnActivate();
            IsActive = true;
            Activated(this, new ActivationEventArgs());
        }

        async Task OnActivate()
        {
            ActivateLicense();
            LoadAppVersion();
            SetCopyrightText();
            await LoadVersions();
        }

        void ActivateLicense()
        {
            if (License != null)
            {
                ((IActivate)License).Activate();
            }
        }

        void LoadAppVersion()
        {
            var version = typeof(App).Assembly.GetAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            var versionParts = version.Split('+');
            var appVersion = versionParts[0];

            AppVersion = appVersion;

            var metadata = versionParts.Last();
            var parts = metadata.Split('.');
            var shaIndex = parts.IndexOf("Sha", StringComparer.InvariantCultureIgnoreCase);
            if (shaIndex != -1 && parts.Length > shaIndex + 1)
            {
                var shaValue = parts[shaIndex + 1];
                var shortCommitHash = shaValue.Substring(0, 7);
                CommitHash = shortCommitHash;
            }
        }

        void SetCopyrightText()
        {
            CopyrightText = string.Format("Copyright 2013-{0} NServiceBus Ltd. All rights reserved.", DateTime.Now.Year);
        }

        async Task LoadVersions()
        {
            if (serviceControl == null)
                return;

            ServiceControlVersion = DetectingServiceControlVersion;

            var version = await serviceControl.GetVersion();
            ServiceControlVersion = version ?? NotConnectedToServiceControl;
        }
    }
}