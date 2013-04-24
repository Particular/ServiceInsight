using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.PresentationFramework.Filters;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Common.Settings;
using NServiceBus.Profiler.Core.Management;
using NServiceBus.Profiler.Core.Settings;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public class ManagementConnectionViewModel : Screen
    {
        private readonly IManagementService _managementService;
        private readonly ISettingsProvider _settingsProvider;
        private readonly ProfilerSettings _appSettings;

        public ManagementConnectionViewModel(
            IManagementService managementService, 
            ISettingsProvider settingsProvider)
        {
            _managementService = managementService;
            _settingsProvider = settingsProvider;
            _appSettings = settingsProvider.GetSettings<ProfilerSettings>();
            DisplayName = "Connect To Management Service";
        }

        public string ServiceUrl { get; set; }
        
        public bool IsAddressValid { get; set; }

        public bool WorkInProgress { get; private set; }

        protected override void OnActivate()
        {
            base.OnActivate();

            IsAddressValid = true;
            ServiceUrl = _appSettings.LastUsedManagementApi;
            RecentEntries = GetRecentServiceEntries();
        }

        private List<string> GetRecentServiceEntries()
        {
            return _appSettings.RecentManagementApiEntries ?? new List<string>();
        }

        public virtual void Close()
        {
            TryClose(false);
        }

        public virtual bool CanAccept()
        {
            return !string.IsNullOrEmpty(ServiceUrl);
        }

        public List<string> RecentEntries { get; private set; }

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
            var existingEntry = _appSettings.RecentManagementApiEntries.Find(x => x.Equals(ServiceUrl, StringComparison.InvariantCultureIgnoreCase));
            if (existingEntry != null)
                _appSettings.RecentManagementApiEntries.Remove(existingEntry);

            _appSettings.RecentManagementApiEntries.Add(ServiceUrl);
            _appSettings.LastUsedManagementApi = ServiceUrl;

            _settingsProvider.SaveSettings(_appSettings);
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