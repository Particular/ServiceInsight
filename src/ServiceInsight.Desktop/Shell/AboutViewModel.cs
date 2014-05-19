namespace Particular.ServiceInsight.Desktop.Shell
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Core;
    using ExceptionHandler;
    using ExtensionMethods;
    using ServiceControl;

    public class AboutViewModel : INotifyPropertyChanged, IActivate, IHaveDisplayName
    {
        public const string DetectingServiceControlVersion = "(Detecting...)";
        public const string NotConnectedToServiceControl = "(Not Connected)";

        NetworkOperations networkOperations;
        DefaultServiceControl serviceControl;

        public event EventHandler<ActivationEventArgs> Activated = delegate { };

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public bool IsSplash { get; set; }

        public bool HasFullLicense { get { return License != null && !License.HasTrialLicense; } }

        public LicenseRegistrationViewModel License { get; private set; }

        public string AppVersion { get; set; }

        public string ServiceControlVersion { get; set; }

        public string DisplayName { get; set; }

        public string CommitHash { get; set; }

        public bool IsActive { get; private set; }

        public AboutViewModel(
            NetworkOperations networkOperations,
            DefaultServiceControl serviceControl,
            LicenseRegistrationViewModel licenseInfo)
        {
            this.networkOperations = networkOperations;
            this.serviceControl = serviceControl;

            License = licenseInfo;
            IsSplash = false;
            DisplayName = "About";
        }

        AboutViewModel()
        {
            IsSplash = true;
        }

        public static AboutViewModel AsSplashScreenModel()
        {
            var vm = new AboutViewModel();
            vm.LoadAppVersion();
            return vm;
        }

        async Task OnActivate()
        {
            ActivateLicense();
            LoadAppVersion();
            await LoadVersions();
        }

        public void NavigateToSite()
        {
            if (IsSplash) return;

            var supportUrl = GetType().Assembly.GetAttribute<SupportWebUrlAttribute>();
            networkOperations.Browse(supportUrl.WebUrl);
        }

        public async void Activate()
        {
            await OnActivate();
            IsActive = true;
            Activated(this, new ActivationEventArgs());
        }

        void LoadAppVersion()
        {
            var version = typeof(App).Assembly.GetAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            var versionParts = version.Split(' ');
            var appVersion = versionParts[0];
            var commitHash = versionParts.Last();

            AppVersion = appVersion;
            CommitHash = GetShortCommitHash(commitHash);
        }

        static string GetShortCommitHash(string commitHash)
        {
            var parts = commitHash.Split(':');
            var shaValue = parts[1].Replace("'", "");
            var shortCommitHash = shaValue.Substring(0, 7);

            return shortCommitHash;
        }

        void ActivateLicense()
        {
            if (License != null)
            {
                ((IActivate)License).Activate();
            }
        }

        async Task LoadVersions()
        {
            ServiceControlVersion = DetectingServiceControlVersion;

            var version = await serviceControl.GetVersion();
            ServiceControlVersion = version ?? NotConnectedToServiceControl;
        }
    }
}