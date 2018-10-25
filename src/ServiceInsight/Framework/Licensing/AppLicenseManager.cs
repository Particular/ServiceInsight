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
        }

        public LicenseInstallationResult TryInstallLicense(string licenseText)
        {
            ValidationResult = LicenseDialogSource.Validate(licenseText);
            if (ValidationResult.License != null)
            {
                if (IsLicenseExpired(ValidationResult.License))
                {
                    return LicenseInstallationResult.Expired;
                }

                new RegistryLicenseStore().StoreLicense(licenseText);
                new FilePathLicenseStore().StoreLicense(FilePathLicenseStore.UserLevelLicenseLocation, licenseText);

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

        public bool HasTrialLicense => CurrentLicense == null || CurrentLicense.IsTrialLicense;

        public bool HasFullLicense => CurrentLicense != null && CurrentLicense.IsCommercialLicense;

        public bool CanExtendTrial => CurrentLicense == null || (CurrentLicense.IsTrialLicense && !CurrentLicense.IsExtendedTrial);

        public int GetRemainingTrialDays()
        {
            var isTrial = CurrentLicense == null || CurrentLicense.ExpirationDate == null;
            var effectiveDate = isTrial ? TrialStartDateStore.GetTrialStartDate().AddDays(14) : CurrentLicense.ExpirationDate.Value;

            var remainingDays = CalcRemainingDays(effectiveDate);

            return remainingDays > 0 ? remainingDays : 0;
        }

        private DateExpirationStatus GetDateStatus(int? remainingDays)
        {
            if (!remainingDays.HasValue)
            {
                return DateExpirationStatus.NotSet;
            }

            if (remainingDays == 0)
            {
                return DateExpirationStatus.ExpiringToday;
            }

            if (remainingDays < 0)
            {
                return DateExpirationStatus.Expired;
            }

            if (remainingDays <= 10)
            {
                return DateExpirationStatus.Expiring;
            }

            return DateExpirationStatus.NotExpired;
        }

        public DateExpirationStatus GetExpirationStatus()
        {
            var remainingDays = GetExpirationRemainingDays();
            return GetDateStatus(remainingDays);
        }

        public DateExpirationStatus GetUpgradeProtectionStatus()
        {
            var remainingDays = GetUpgradeProtectionRemainingDays();
            return GetDateStatus(remainingDays);
        }

        public int? GetExpirationRemainingDays()
        {
            if (!CurrentLicense.ExpirationDate.HasValue)
            {
                return null;
            }

            return CalcRemainingDays(CurrentLicense.ExpirationDate.Value);
        }

        public int? GetUpgradeProtectionRemainingDays()
        {
            if (!CurrentLicense.UpgradeProtectionExpiration.HasValue)
            {
                return null;
            }

            return CalcRemainingDays(CurrentLicense.UpgradeProtectionExpiration.Value);
        }

        public bool IsLicenseExpired() => LicenseExpirationChecker.HasLicenseExpired(CurrentLicense);

        private bool IsLicenseExpired(License license) => LicenseExpirationChecker.HasLicenseExpired(license);

        private static int CalcRemainingDays(DateTimeOffset date)
        {
            var oneDayGrace = date;

            if (date < DateTime.MaxValue.AddDays(-1))
            {
                oneDayGrace = date.AddDays(1);
            }

            var now = DateTime.UtcNow.Date;
            var remainingDays = (oneDayGrace - now).Days;

            return remainingDays;
        }
    }
}