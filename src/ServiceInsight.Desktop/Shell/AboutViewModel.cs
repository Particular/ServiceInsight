namespace Particular.ServiceInsight.Desktop.Shell
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Screens;
    using Core;
    using ExceptionHandler;
    using ExtensionMethods;
    using ServiceControl;

    public class AboutViewModel : INotifyPropertyChanged, IActivate, IHaveDisplayName
    {
        public const string DetectingServiceControlVersion = "(Detecting...)";
        public const string NotConnectedToServiceControl = "(Not Connected)";

        private readonly INetworkOperations networkOperations;
        private readonly IServiceControl serviceControl;

        public event EventHandler<ActivationEventArgs> Activated = delegate { };
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public bool IsSplash { get; set; }
        public bool HasFullLicense { get { return License != null && !License.HasTrialLicense; } }
        public ILicenseRegistrationViewModel License { get; private set; }
        public string AppVersion { get; set; }
        public string ServiceControlVersion { get; set; }
        public string DisplayName { get; set; }
        public string CommitHash { get; set; }
        public bool IsActive { get; private set; }

        public AboutViewModel(
            INetworkOperations networkOperations, 
            IServiceControl serviceControl,
            ILicenseRegistrationViewModel licenseInfo)
        {
            this.networkOperations = networkOperations;
            this.serviceControl = serviceControl;
            
            License = licenseInfo;
            IsSplash = false;
            DisplayName = "About";
        }

        private AboutViewModel()
        {
            IsSplash = true;
        }

        public static AboutViewModel AsSplashScreenModel()
        {
            var vm = new AboutViewModel();
            vm.LoadAppVersion();
            return vm;
        }

        private async Task OnActivate()
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

        private void LoadAppVersion()
        {
            var version = typeof(App).Assembly.GetAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            var versionParts = version.Split(' ');
            var appVersion = versionParts[0];
            var commitHash = versionParts.Last();

            AppVersion = appVersion;
            CommitHash = GetShortCommitHash(commitHash);
        }

        private static string GetShortCommitHash(string commitHash)
        {
            var parts = commitHash.Split(':');
            var shaValue = parts[1].Replace("'", "");
            var shortCommitHash = shaValue.Substring(0, 7);

            return shortCommitHash;
        }

        private void ActivateLicense()
        {
            if (License != null)
            {
                License.Activate();
            }
        }

        private async Task LoadVersions()
        {
            ServiceControlVersion = DetectingServiceControlVersion;

            var version = await serviceControl.GetVersion();
            ServiceControlVersion = version ?? NotConnectedToServiceControl;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}