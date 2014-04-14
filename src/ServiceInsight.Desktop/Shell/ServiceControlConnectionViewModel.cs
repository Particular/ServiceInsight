using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Caliburn.PresentationFramework.Filters;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Core.Settings;
using NServiceBus.Profiler.Desktop.ServiceControl;
using NServiceBus.Profiler.Desktop.Settings;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public class ServiceControlConnectionViewModel : Screen
    {
        public const string ConnectingToServiceControl = "Connecting to ServiceControl...";

        private readonly ISettingsProvider _settingsProvider;
        private readonly ProfilerSettings _appSettings;
        private readonly IContainer _container;

        public ServiceControlConnectionViewModel(
            ISettingsProvider settingsProvider,
            IContainer container)
        {
            _settingsProvider = settingsProvider;
            _container = container;
            _appSettings = settingsProvider.GetSettings<ProfilerSettings>();
            DisplayName = "Connect To ServiceControl";
        }

        public string ServiceUrl { get; set; }
        
        public bool IsAddressValid { get; set; }

        public bool WorkInProgress { get; private set; }

        protected override void OnActivate()
        {
            base.OnActivate();

            IsAddressValid = true;
            ServiceUrl = _appSettings.LastUsedServiceControl;
            RecentEntries = GetRecentServiceEntries();
        }

        private List<string> GetRecentServiceEntries()
        {
            return _appSettings.RecentServiceControlEntries.ToList();
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

        public string Version { get; private set; }

        [AutoCheckAvailability]
        public async virtual void Accept()
        {
            StartWorkInProgress();
            IsAddressValid = await IsValidUrl(ServiceUrl);
            if (IsAddressValid)
            {
                StoreConnectionAddress();
                TryClose(true);
            }
            StopWorkInProgress();
        }

        private void StartWorkInProgress()
        {
            ProgressMessage = ConnectingToServiceControl;
            WorkInProgress = true;
        }

        private void StopWorkInProgress()
        {
            ProgressMessage = string.Empty;
            WorkInProgress = false;
        }

        public string ProgressMessage
        {
            get; set;
        }

        private void StoreConnectionAddress()
        {
            var existingEntry = _appSettings.RecentServiceControlEntries.FirstOrDefault(x => x.Equals(ServiceUrl, StringComparison.InvariantCultureIgnoreCase));
            if (existingEntry != null)
                _appSettings.RecentServiceControlEntries.Remove(existingEntry);

            _appSettings.RecentServiceControlEntries.Add(ServiceUrl);
            _appSettings.LastUsedServiceControl = ServiceUrl;

            _settingsProvider.SaveSettings(_appSettings);
        }

        private async Task<bool> IsValidUrl(string serviceUrl)
        {
            if (Uri.IsWellFormedUriString(serviceUrl, UriKind.Absolute))
            {
                using (var scope = _container.BeginLifetimeScope())
                {
                    var connection = scope.Resolve<IServiceControlConnectionProvider>();
                    var service = scope.Resolve<IServiceControl>();

                    connection.ConnectTo(serviceUrl);
                    Version = await service.GetVersion();

                    return Version != null;
                }
            }

            return false;
        }
    }
}