namespace ServiceInsight.Shell
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using ServiceControl;
    using ExtensionMethods;
    using Framework.Settings;
    using Settings;
    using System.Windows.Controls;

    public class ServiceControlConnectionViewModel : Screen
    {
        const string ConnectingToServiceControl = "Connecting to ServiceControl...";
        const string ConnectionErrorMessage = "There was an error connecting to ServiceControl. Either the address is not valid or the service is down.";
        const string CertValidationErrorMessage = "There was an error connecting to ServiceControl. SSL certificate is not valid.";
        const string AddressInvalid = "Entered address is invalid.";
        const string ConnectionExists = "You have already connected to this address.";
        const string AuthenticationFailedMessage = "Authentication failed. Please check your credentials.";

        static bool certValidationFailed;
        static bool authenticationFailed;

        readonly ISettingsProvider settingsProvider;
        readonly ServiceControlClientRegistry clientRegistry;
        readonly ProfilerSettings appSettings;

        static ServiceControlConnectionViewModel()
        {
            DefaultServiceControl.CertificateValidationFailed = () => { certValidationFailed = true; };
            DefaultServiceControl.AuthenticationFailed = () => { authenticationFailed = true; };
        }

        public ServiceControlConnectionViewModel(
            ISettingsProvider settingsProvider,
            ServiceControlClientRegistry clientRegistry)
        {
            this.settingsProvider = settingsProvider;
            this.clientRegistry = clientRegistry;
            appSettings = settingsProvider.GetSettings<ProfilerSettings>();
            DisplayName = "Connect To ServiceControl";
        }

        public string ServiceUrl { get; set; }

        public bool ShowError { get; set; }

        public string ErrorMessage { get; set; }

        public bool WorkInProgress { get; private set; }

        public bool UseWindowsAuthWithCustomUsernamePassword { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public void PasswordChanged(PasswordBox passwordBox)
        {
            Password = passwordBox.Password;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            ShowError = false;
            ServiceUrl = appSettings.LastUsedServiceControl;
            UseWindowsAuthWithCustomUsernamePassword = appSettings.UseWindowsAuthWithCustomUsernamePassword;
            Username = appSettings.WindowsAuthenticationCustomUsername;
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
            certValidationFailed = false;
            StartWorkInProgress();
            ServiceUrl = ServiceUrl.Trim();
            var isValidUrl = await IsValid(ServiceUrl);
            ShowError = !isValidUrl;

            if (!ShowError)
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
            appSettings.UseWindowsAuthWithCustomUsernamePassword = UseWindowsAuthWithCustomUsernamePassword;
            appSettings.WindowsAuthenticationCustomUsername = Username;

            settingsProvider.SaveSettings(appSettings);
        }

        async Task<bool> IsValid(string serviceUrl)
        {
            var valid = true;

            if (!serviceUrl.IsValidUrl())
            {
                ErrorMessage = AddressInvalid;
                valid = false;
            }

            if (clientRegistry.IsRegistered(serviceUrl))
            {
                ErrorMessage = ConnectionExists;
                valid = false;
            }

            if (valid)
            {
                var address = serviceUrl;
                var service = clientRegistry.Create(address, UseWindowsAuthWithCustomUsernamePassword ? Username : null, UseWindowsAuthWithCustomUsernamePassword ? Password : null);

                (Version, address) = await service.GetVersion();

                if (Version == null)
                {
                    clientRegistry.RemoveServiceControlClient(address);
                    if (authenticationFailed)
                    {
                        ErrorMessage = AuthenticationFailedMessage;
                    }
                    else if (certValidationFailed)
                    {
                        ErrorMessage = CertValidationErrorMessage;
                    }
                    else
                    {
                        ErrorMessage = ConnectionErrorMessage;
                    }
                    valid = false;
                }
                else
                {
                    clientRegistry.EnsureServiceControlClient(address);
                }
            }

            return valid;
        }
    }
}