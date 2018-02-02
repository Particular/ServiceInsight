namespace ServiceInsight.Shell
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac;
    using Caliburn.Micro;
    using ServiceControl;
    using ServiceInsight.ExtensionMethods;
    using ServiceInsight.Framework.Settings;
    using Settings;

    public class ServiceControlConnectionViewModel : Screen
    {
        public const string ConnectingToServiceControl = "Connecting to ServiceControl...";

        ISettingsProvider settingsProvider;
        ProfilerSettings appSettings;
        IContainer container;

        public ServiceControlConnectionViewModel(
            ISettingsProvider settingsProvider,
            IContainer container)
        {
            this.settingsProvider = settingsProvider;
            this.container = container;
            appSettings = settingsProvider.GetSettings<ProfilerSettings>();
            DisplayName = "Connect To ServiceControl";
        }

        public string ServiceUrl { get; set; }

        public bool IsAddressValid { get; set; }

        public bool WorkInProgress { get; private set; }

        protected override void OnActivate()
        {
            base.OnActivate();

            IsAddressValid = true;
            ServiceUrl = appSettings.LastUsedServiceControl;
            RecentEntries = GetRecentServiceEntries();
        }

        List<string> GetRecentServiceEntries() => appSettings.RecentServiceControlEntries.ToList();

        public virtual void Close()
        {
            TryClose(false);
        }

        public virtual bool CanAccept() => !string.IsNullOrEmpty(ServiceUrl);

        public List<string> RecentEntries { get; private set; }

        public string Version { get; private set; }

        public virtual async Task Accept()
        {
            StartWorkInProgress();
            ServiceUrl = ServiceUrl.Trim();
            IsAddressValid = await IsValidUrl(ServiceUrl);
            if (IsAddressValid)
            {
                StoreConnectionAddress();
                TryClose(true);
            }
            StopWorkInProgress();
        }

        void StartWorkInProgress()
        {
            ProgressMessage = ConnectingToServiceControl;
            WorkInProgress = true;
        }

        void StopWorkInProgress()
        {
            ProgressMessage = string.Empty;
            WorkInProgress = false;
        }

        public string ProgressMessage
        {
            get;
            set;
        }

        void StoreConnectionAddress()
        {
            var existingEntry = appSettings.RecentServiceControlEntries.FirstOrDefault(x => x.Equals(ServiceUrl, StringComparison.InvariantCultureIgnoreCase));
            if (existingEntry != null)
            {
                appSettings.RecentServiceControlEntries.Remove(existingEntry);
            }

            appSettings.RecentServiceControlEntries.Add(ServiceUrl);
            appSettings.LastUsedServiceControl = ServiceUrl;

            settingsProvider.SaveSettings(appSettings);
        }

        async Task<bool> IsValidUrl(string serviceUrl)
        {
            if (serviceUrl.IsValidUrl())
            {
                using (var scope = container.BeginLifetimeScope())
                {
                    var connection = scope.Resolve<ServiceControlConnectionProvider>();
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