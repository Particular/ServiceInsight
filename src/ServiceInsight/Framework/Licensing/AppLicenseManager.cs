namespace ServiceInsight.Framework.Licensing
{
    using System;
    using Anotar.Serilog;
    using Particular.Licensing;

    public class AppLicenseManager
    {
        public AppLicenseManager()
        {
            var sources = LicenseSource.GetStandardLicenseSources().ToArray();
            var result = ActiveLicense.Find("ServiceInsight", sources);
            CurrentLicense = result.License;
            HasEvaluationLicense = string.Equals(result.Location, "Trial License", StringComparison.OrdinalIgnoreCase);
        }

        public LicenseInstallationResult TryInstallLicense(string licenseText)
        {
            ValidationResult = LicenseDialogSource.Validate(licenseText);
            if (ValidationResult.License != null)
            {
                if (ValidationResult.License.HasExpired())
                {
                    return LicenseInstallationResult.Expired;
                }

                new RegistryLicenseStore().StoreLicense(licenseText);
                new FilePathLicenseStore().StoreLicense(LicenseFileLocationResolver.GetPathFor(Environment.SpecialFolder.LocalApplicationData), licenseText);

                CurrentLicense = ValidationResult.License;

                return LicenseInstallationResult.Succeeded;
            }

            LogTo.Warning($"Can't install license: {ValidationResult.Result}");

            return LicenseInstallationResult.Failed;
        }

        internal License CurrentLicense { get; set; }

        internal LicenseSourceResult ValidationResult { get; set; }

        public string LicenseType => CurrentLicense?.LicenseType;

        public string RegisteredTo => CurrentLicense?.RegisteredTo;

        public bool HasNonProductionLicense => CurrentLicense == null || CurrentLicense.IsTrialLicense;

        public bool HasFullLicense => CurrentLicense != null && CurrentLicense.IsCommercialLicense;

        public bool HasEvaluationLicense { get; set; }

        public bool CanExtendLicense => CurrentLicense == null || (CurrentLicense.IsTrialLicense && !CurrentLicense.IsExtendedTrial);

        public int GetRemainingNonProductionDays()
        {
            var remaining = CurrentLicense.GetDaysUntilLicenseExpires().GetValueOrDefault(0);
            return Math.Max(remaining, 0);
        }

        public DateExpirationStatus GetExpirationStatus()
        {
            var status = CurrentLicense.GetLicenseStatus();
            switch (status)
            {
                case LicenseStatus.Valid:
                    return DateExpirationStatus.NotExpired;
                case LicenseStatus.ValidWithExpiringSubscription:
                    return DateExpirationStatus.Expiring;
                case LicenseStatus.InvalidDueToExpiredSubscription:
                    return DateExpirationStatus.Expired;
                case LicenseStatus.ValidWithExpiredUpgradeProtection:
                case LicenseStatus.ValidWithExpiringTrial:
                case LicenseStatus.ValidWithExpiringUpgradeProtection:
                case LicenseStatus.InvalidDueToExpiredTrial:
                case LicenseStatus.InvalidDueToExpiredUpgradeProtection:
                default:
                    return DateExpirationStatus.NotSet;
            }
        }

        public DateExpirationStatus GetUpgradeProtectionStatus()
        {
            var status = CurrentLicense.GetLicenseStatus();
            switch (status)
            {
                case LicenseStatus.Valid:
                    return DateExpirationStatus.NotExpired;
                case LicenseStatus.ValidWithExpiredUpgradeProtection:
                    return DateExpirationStatus.Expired;
                case LicenseStatus.ValidWithExpiringUpgradeProtection:
                    return DateExpirationStatus.Expiring;
                case LicenseStatus.InvalidDueToExpiredUpgradeProtection:
                    return DateExpirationStatus.Expired;
                case LicenseStatus.ValidWithExpiringTrial:
                case LicenseStatus.ValidWithExpiringSubscription:
                case LicenseStatus.InvalidDueToExpiredTrial:
                case LicenseStatus.InvalidDueToExpiredSubscription:
                default:
                    return DateExpirationStatus.NotSet;
            }
        }

        public DateExpirationStatus GetNonProductionExpirationStatus()
        {
            var status = CurrentLicense.GetLicenseStatus();
            switch (status)
            {
                case LicenseStatus.Valid:
                    return DateExpirationStatus.NotExpired;
                case LicenseStatus.ValidWithExpiringTrial:
                    return DateExpirationStatus.Expiring;
                case LicenseStatus.InvalidDueToExpiredTrial:
                    return DateExpirationStatus.Expired;
                case LicenseStatus.ValidWithExpiredUpgradeProtection:
                case LicenseStatus.ValidWithExpiringSubscription:
                case LicenseStatus.ValidWithExpiringUpgradeProtection:
                case LicenseStatus.InvalidDueToExpiredSubscription:
                case LicenseStatus.InvalidDueToExpiredUpgradeProtection:
                default:
                    return DateExpirationStatus.NotSet;
            }
        }

        public int? GetExpirationRemainingDays()
        {
            return CurrentLicense.GetDaysUntilLicenseExpires();
        }

        public int? GetUpgradeProtectionRemainingDays()
        {
            return CurrentLicense.GetDaysUntilUpgradeProtectionExpires();
        }

        public bool IsLicenseExpired() => CurrentLicense.HasExpired();
    }
}