using System.Reflection;
using System.Threading.Tasks;
using Caliburn.PresentationFramework.Screens;
using ExceptionHandler;
using NServiceBus.Profiler.Common.ExtensionMethods;
using NServiceBus.Profiler.Core;
using NServiceBus.Profiler.Core.Management;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public class AboutViewModel : Screen
    {
        private readonly INetworkOperations _networkOperations;
        private readonly IManagementService _managementService;

        public AboutViewModel(
            INetworkOperations networkOperations, 
            IManagementService managementService)
        {
            _networkOperations = networkOperations;
            _managementService = managementService;
            DisplayName = "About";
        }

        protected async override void OnActivate()
        {
            base.OnActivate();
            await LoadVersions();
        }

        private async Task LoadVersions()
        {
            AppVersion = typeof(App).Assembly.GetAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            ManagementApiVersion = "(Detecting...)";

            var version = await _managementService.GetVersion();
            if (version != null)
            {
                ManagementApiVersion = version.Version;
            }
            else
            {
                ManagementApiVersion = "(Not Connected)";
            }
        }

        public void NavigateToSite()
        {
            var supportUrl = GetType().Assembly.GetAttribute<SupportWebUrlAttribute>();
            _networkOperations.Browse(supportUrl.WebUrl);
        }

        public string AppVersion { get; set; }
        public string ManagementApiVersion { get; set; }
    }
}