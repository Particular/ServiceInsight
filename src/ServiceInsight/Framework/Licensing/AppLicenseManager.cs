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
            if (ValidationResult.License != null)
            {
                new RegistryLicenseStore().StoreLicense(licenseText);
                new FilePathLicenseStore().StoreLicense(FilePathLicenseStore.UserLevelLicenseLocation, licenseText);

                return true;
            }

            LogTo.Warning($"Can't install license: {ValidationResult.Result}");
            return false;
        }

        internal License CurrentLicense { get; set; }

        internal LicenseSourceResult ValidationResult { get; set; }

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

        private static int CalcRemainingDays(DateTimeOffset date)
        {
            var now = DateTime.UtcNow.Date;
            var dayAfterExpiration = date.AddDays(1);
            var remainingDays = (dayAfterExpiration - now).Days;

            return remainingDays > 0 ? remainingDays : 0;
        }
    }
}