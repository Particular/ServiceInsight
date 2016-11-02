namespace ServiceInsight.Shell
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Windows.Input;
    using Autofac;
    using ExtensionMethods;
    using Framework.Rx;
    using Pirac;
    using ServiceControl;
    using ServiceInsight.Framework.Settings;
    using Settings;

    public class ServiceControlConnectionViewModel : RxScreen
    {
        public const string ConnectingToServiceControl = "Connecting to ServiceControl...";

        ISettingsProvider settingsProvider;
        ProfilerSettings appSettings;
        Autofac.IContainer container;

        public ServiceControlConnectionViewModel(
            ISettingsProvider settingsProvider,
            Autofac.IContainer container)
        {
            this.settingsProvider = settingsProvider;
            this.container = container;
            appSettings = settingsProvider.GetSettings<ProfilerSettings>();
            DisplayName = "Connect To ServiceControl";

            AcceptCommand = this.WhenPropertiesChanged<string>(nameof(ServiceUrl))
                //.Select(pcd => !pcd.After.IsEmpty())
                .Select(_ => !ServiceUrl.IsEmpty())
                .ToCommand(_ => Accept());
            CancelCommand = Command.Create(() => TryClose(false));
        }

        public ICommand AcceptCommand { get; }

        public ICommand CancelCommand { get; }

        public string ServiceUrl { get; set; }

        public bool IsAddressValid { get; set; }

        public bool WorkInProgress { get; private set; }

        protected override void OnActivate(bool wasInitialized)
        {
            IsAddressValid = true;
            ServiceUrl = appSettings.LastUsedServiceControl;
            RecentEntries = GetRecentServiceEntries();
        }

        List<string> GetRecentServiceEntries() => appSettings.RecentServiceControlEntries.ToList();

        public List<string> RecentEntries { get; private set; }

        public string Version { get; private set; }

        void Accept()
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
            {
                appSettings.RecentServiceControlEntries.Remove(existingEntry);
            }

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