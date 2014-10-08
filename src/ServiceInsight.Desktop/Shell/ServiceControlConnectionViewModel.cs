namespace Particular.ServiceInsight.Desktop.Shell
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;
    using Caliburn.Micro;
    using Core.Settings;
    using ServiceControl;
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

        List<string> GetRecentServiceEntries()
        {
            return appSettings.RecentServiceControlEntries.ToList();
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

        public virtual void Accept()
        {
            StartWorkInProgress();
            ServiceUrl = ServiceUrl.Trim();
            IsAddressValid = IsValidUrl(ServiceUrl);
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
                appSettings.RecentServiceControlEntries.Remove(existingEntry);

            appSettings.RecentServiceControlEntries.Add(ServiceUrl);
            appSettings.LastUsedServiceControl = ServiceUrl;

            settingsProvider.SaveSettings(appSettings);
        }

        bool IsValidUrl(string serviceUrl)
        {
            if (Uri.IsWellFormedUriString(serviceUrl, UriKind.Absolute))
            {
                using (var scope = container.BeginLifetimeScope())
                {
                    var connection = scope.Resolve<ServiceControlConnectionProvider>();
                    var service = scope.Resolve<IServiceControl>();

                    connection.ConnectTo(serviceUrl);
                    Version = service.GetVersion();

                    return Version != null;
                }
            }

            return false;
        }
    }
}