using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Caliburn.PresentationFramework.Filters;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Common.Settings;
using NServiceBus.Profiler.Core.Settings;
using NServiceBus.Profiler.Desktop.Management;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public class ManagementConnectionViewModel : Screen
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly ProfilerSettings _appSettings;
        private readonly IContainer _container;

        public ManagementConnectionViewModel(
            ISettingsProvider settingsProvider,
            IContainer container)
        {
            _settingsProvider = settingsProvider;
            _container = container;
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

        public VersionInfo Version { get; private set; }

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
                using (var scope = _container.BeginLifetimeScope())
                {
                    var connection = scope.Resolve<IManagementConnectionProvider>();
                    var service = scope.Resolve<IManagementService>();

                    connection.ConnectTo(serviceUrl);
                    Version = await service.GetVersion();

                    return Version != null;
                }
            }

            return false;
        }
    }
}