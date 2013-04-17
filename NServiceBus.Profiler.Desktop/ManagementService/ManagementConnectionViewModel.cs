using System;
using System.Threading.Tasks;
using Caliburn.PresentationFramework.Filters;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Common.Settings;
using NServiceBus.Profiler.Core.Management;
using NServiceBus.Profiler.Core.Settings;

namespace NServiceBus.Profiler.Desktop.ManagementService
{
    public class ManagementConnectionViewModel : Screen
    {
        private readonly IManagementService _managementService;
        private readonly ISettingsProvider _settingsProvider;

        public ManagementConnectionViewModel(IManagementService managementService, ISettingsProvider settingsProvider)
        {
            _managementService = managementService;
            _settingsProvider = settingsProvider;
            DisplayName = "Connect To Service";
        }

        public string ServiceUrl { get; set; }
        
        public bool IsAddressValid { get; set; }

        public bool WorkInProgress { get; private set; }

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
                StoreConnectionAddress();
                TryClose(true);
            }
            WorkInProgress = false;
        }

        private void StoreConnectionAddress()
        {
            var settings = _settingsProvider.GetSettings<ProfilerSettings>();
            settings.LastUsedManagementApi = ServiceUrl;
            _settingsProvider.SaveSettings(settings);
        }

        private async Task<bool> IsValidUrl(string serviceUrl)
        {
            if (Uri.IsWellFormedUriString(serviceUrl, UriKind.Absolute))
            {
                return await _managementService.IsAlive(serviceUrl);   
            }

            return false;
        }
    }
}