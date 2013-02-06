using Caliburn.PresentationFramework.Filters;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Core.Management;

namespace NServiceBus.Profiler.Desktop.ManagementService
{
    public class ManagementConnectionViewModel : Screen
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
        public virtual void Accept()
        {
            IsAddressValid = IsValidUrl(ServiceUrl);
            if (IsAddressValid)
            {
                TryClose(true);
            }
        }

        private bool IsValidUrl(string serviceUrl)
        {
            return _managementService.IsAlive(serviceUrl);
        }
    }
}