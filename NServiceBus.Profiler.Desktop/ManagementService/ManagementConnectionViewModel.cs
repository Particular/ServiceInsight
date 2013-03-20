using System;
using System.Threading.Tasks;
using Caliburn.PresentationFramework.Filters;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Core.Management;
using NServiceBus.Profiler.Desktop.Shell;

namespace NServiceBus.Profiler.Desktop.ManagementService
{
    public class ManagementConnectionViewModel : Screen, IWorkTracker
    {
        private readonly IManagementService _managementService;

        public ManagementConnectionViewModel(IManagementService managementService)
        {
            _managementService = managementService;
            DisplayName = "Connect To Service";
        }

        public string ServiceUrl { get; set; }
        
        public bool IsAddressValid { get; set; }

        protected override void OnActivate()
        {
            base.OnActivate();

            IsAddressValid = true;
        }

        public virtual void Close()
        {
            TryClose(false);
        }

        public virtual bool CanAccept()
        {
            return !string.IsNullOrEmpty(ServiceUrl);
        }

        [AutoCheckAvailability]
        public async virtual void Accept()
        {
            WorkInProgress = true;
            IsAddressValid = await IsValidUrl(ServiceUrl);
            if (IsAddressValid)
            {
                TryClose(true);
            }
            WorkInProgress = false;
        }

        private async Task<bool> IsValidUrl(string serviceUrl)
        {
            if (Uri.IsWellFormedUriString(serviceUrl, UriKind.Absolute))
            {
                return await _managementService.IsAlive(serviceUrl);   
            }

            return false;
        }

        public bool WorkInProgress { get; private set; }
    }
}