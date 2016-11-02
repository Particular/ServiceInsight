namespace ServiceInsight.Shell
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Input;
    using ExtensionMethods;
    using Framework;
    using Pirac;
    using ServiceControl;

    public class AboutViewModel : BindableObject
    {
        const string DetectingServiceControlVersion = "(Detecting...)";
        const string NotConnectedToServiceControl = "(Not Connected)";

        IServiceControl serviceControl;

        public AboutViewModel(
            NetworkOperations networkOperations,
            IServiceControl serviceControl,
            LicenseRegistrationViewModel licenseInfo)
        {
            this.serviceControl = serviceControl;
            License = licenseInfo;

            IsSplash = false;
            NavigateToSiteCommand = Command.Create(() => networkOperations.Browse("http://www.particular.net"));

            Initialize();
        }

        public AboutViewModel()
        {
            IsSplash = true;
            NavigateToSiteCommand = Command.Create(() => { }, () => false);

            Initialize();
        }

        public bool IsSplash { get; }

        public bool HasFullLicense => License != null && !License.HasTrialLicense;

        public LicenseRegistrationViewModel License { get; }

        public string AppVersion { get; private set; }

        public string ServiceControlVersion { get; private set; }

        public string CopyrightText { get; private set; }

        public string CommitHash { get; private set; }

        public ICommand NavigateToSiteCommand { get; }

        void Initialize()
        {
            ActivateLicense();
            LoadAppVersion();
            SetCopyrightText();
            LoadVersions();
        }

        void ActivateLicense()
        {
            if (License != null)
            {
                License.Initialize();
            }
        }

        void LoadAppVersion()
        {
            var assemblyInfo = typeof(App).Assembly.GetAttribute<AssemblyInformationalVersionAttribute>();
            var version = assemblyInfo != null ? assemblyInfo.InformationalVersion : "Unknown Version";
            var versionParts = version.Split('+');
            var appVersion = versionParts[0];

            AppVersion = appVersion;

            var metadata = versionParts.Last();
            var parts = metadata.Split('.');
            var shaIndex = parts.IndexOf("Sha", StringComparer.InvariantCultureIgnoreCase);
            if (shaIndex != -1 && parts.Length > shaIndex + 1)
            {
                var shaValue = parts[shaIndex + 1];
                var shortCommitHash = shaValue.Substring(0, 7);
                CommitHash = shortCommitHash;
            }
        }

        void SetCopyrightText()
        {
            CopyrightText = string.Format("Copyright 2013-{0} NServiceBus Ltd. All rights reserved.", DateTime.Now.Year);
        }

        void LoadVersions()
        {
            if (serviceControl == null)
            {
                return;
            }

            ServiceControlVersion = DetectingServiceControlVersion;
            var version = serviceControl.GetVersion();
            ServiceControlVersion = version ?? NotConnectedToServiceControl;
        }
    }
}