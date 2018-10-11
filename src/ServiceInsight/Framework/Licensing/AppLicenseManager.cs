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

        public bool TryInstallLicense(string licenseText)
        {
            ValidationResult = LicenseDialogSource.Validate(licenseText);
            if (ValidationResult.License != null && !IsLicenseExpired(ValidationResult.License))
            {
                new RegistryLicenseStore().StoreLicense(licenseText);
                new FilePathLicenseStore().StoreLicense(FilePathLicenseStore.UserLevelLicenseLocation, licenseText);

                CurrentLicense = ValidationResult.License;

                return true;
            }

            LogTo.Warning($"Can't install license: {ValidationResult.Result}");
            return false;
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

            return CalcRemainingDays(effectiveDate);
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

            return remainingDays > 0 ? remainingDays : 0;
        }
    }
}