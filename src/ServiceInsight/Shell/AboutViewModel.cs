namespace ServiceInsight.Shell
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Caliburn.Micro;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.Licensing;
    using ServiceInsight.ServiceControl;

    public class AboutViewModel : INotifyPropertyChanged, IActivate, IHaveDisplayName
    {
        const string DetectingServiceControlVersion = "(Detecting...)";
        const string NotConnectedToServiceControl = "(Not Connected)";

        IApplicationVersionService applicationVersionService;
        IServiceControl serviceControl;
        private AppLicenseManager licenseManager;

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
            IServiceControl serviceControl,
            AppLicenseManager licenseManager)
        {
            this.applicationVersionService = applicationVersionService;
            this.serviceControl = serviceControl;

            this.licenseManager = licenseManager;
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
            CopyrightText = string.Format("Copyright 2013-{0} NServiceBus Ltd. All rights reserved.", DateTime.Now.Year);
        }

        async Task LoadVersions()
        {
            if (serviceControl == null)
            {
                return;
            }

            ServiceControlVersion = DetectingServiceControlVersion;

            var version = await serviceControl.GetVersion();
            ServiceControlVersion = version ?? NotConnectedToServiceControl;
        }
    }
}