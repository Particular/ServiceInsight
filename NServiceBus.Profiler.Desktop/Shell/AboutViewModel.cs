using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Caliburn.PresentationFramework.Screens;
using ExceptionHandler;
using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.ExtensionMethods;
using NServiceBus.Profiler.Desktop.ServiceControl;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public class AboutViewModel : INotifyPropertyChanged, IActivate
    {
        private readonly INetworkOperations _networkOperations;
        private readonly IServiceControl _serviceControl;

        public event EventHandler<ActivationEventArgs> Activated = delegate { };
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public bool IsSplash { get; set; }
        public bool HasFullLicense { get { return License != null && !License.HasTrialLicense; } }
        public ILicenseRegistrationViewModel License { get; private set; }
        public string AppVersion { get; set; }
        public string ServiceControlVersion { get; set; }
        public bool IsActive { get; private set; }

        public AboutViewModel(
            INetworkOperations networkOperations, 
            IServiceControl serviceControl,
            ILicenseRegistrationViewModel licenseInfo)
        {
            _networkOperations = networkOperations;
            _serviceControl = serviceControl;
            
            License = licenseInfo;
            IsSplash = false;
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
            _networkOperations.Browse(supportUrl.WebUrl);
        }

        public async void Activate()
        {
            await OnActivate();
            IsActive = true;
            Activated(this, new ActivationEventArgs());
        }

        private void LoadAppVersion()
        {
            AppVersion = typeof(App).Assembly.GetAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
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
            ServiceControlVersion = "(Detecting...)";

            var version = await _serviceControl.GetVersion();
            ServiceControlVersion = version ?? "(Not Connected)";
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}