namespace ServiceInsight.Shell
{
    using System.Linq;
    using ServiceInsight.ServiceControl;
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Caliburn.Micro;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.Licensing;

    public class AboutViewModel : INotifyPropertyChanged, IActivate, IHaveDisplayName
    {
        internal const string DetectingServiceControlVersion = "(Detecting...)";
        internal const string NotConnectedToServiceControl = "(Not Connected)";
        internal const string MultipleConnection = "(Multiple Connections)";

        readonly IApplicationVersionService applicationVersionService;
        readonly AppLicenseManager licenseManager;
        readonly ServiceControlClientRegistry clientRegistry;

        public event EventHandler<ActivationEventArgs> Activated = (s, e) => { };

        public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

        public bool IsSplash { get; }

        public bool ShowRegisteredTo => RegisteredTo != null;

        public string RegisteredTo => licenseManager?.RegisteredTo;

        public string AppVersion { get; private set; }

        public string ServiceControlVersion { get; private set; }

        public string CopyrightText { get; private set; }

        public string DisplayName { get; set; }

        public string CommitHash { get; private set; }

        public bool IsActive { get; private set; }

        public ICommand NavigateToSiteCommand { get; }

        public AboutViewModel(
            NetworkOperations networkOperations,
            IApplicationVersionService applicationVersionService,
            AppLicenseManager licenseManager,
            ServiceControlClientRegistry clientRegistry)
        {
            this.applicationVersionService = applicationVersionService;
            this.licenseManager = licenseManager;
            this.clientRegistry = clientRegistry;
            IsSplash = false;
            DisplayName = "About";

            NavigateToSiteCommand = Command.Create(() => networkOperations.Browse("http://www.particular.net"));
        }

        AboutViewModel(IApplicationVersionService applicationVersionService)
        {
            IsSplash = true;
            this.applicationVersionService = applicationVersionService;
        }

        public static AboutViewModel AsSplashScreenModel()
        {
            var vm = new AboutViewModel(new ApplicationVersionService());
            vm.Activate();
            return vm;
        }

        public void Activate()
        {
            OnActivate();
            IsActive = true;
            Activated(this, new ActivationEventArgs());
        }

        async void OnActivate()
        {
            LoadAppVersion();
            SetCopyrightText();
            await LoadVersions();
        }

        void LoadAppVersion()
        {
            AppVersion = applicationVersionService.GetVersion();
            CommitHash = applicationVersionService.GetCommitHash();
        }

        void SetCopyrightText()
        {
#pragma warning disable PS0023 // DateTime.UtcNow or DateTimeOffset.UtcNow should be used instead of DateTime.Now and DateTimeOffset.Now, unless the value is being used for displaying the current date-time in a user's local time zone
            CopyrightText = string.Format("Copyright 2013-{0} NServiceBus Ltd. All rights reserved.", DateTime.Now.Year);
#pragma warning restore PS0023 // DateTime.UtcNow or DateTimeOffset.UtcNow should be used instead of DateTime.Now and DateTimeOffset.Now, unless the value is being used for displaying the current date-time in a user's local time zone
        }

        async Task LoadVersions()
        {
            if (clientRegistry == null)
            {
                return;
            }

            ServiceControlVersion = DetectingServiceControlVersion;
            var versions = (await clientRegistry.GetVersions()).ToList();

            if (versions.Count == 0)
            {
                ServiceControlVersion = NotConnectedToServiceControl;
            }
            else if (versions.Count == 1)
            {
                ServiceControlVersion = versions[0];
            }
            else
            {
                ServiceControlVersion = MultipleConnection;
            }
        }
    }
}