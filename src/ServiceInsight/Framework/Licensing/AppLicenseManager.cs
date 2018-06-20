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
            var now = DateTime.UtcNow.Date;
            var expiration = (CurrentLicense == null || CurrentLicense.ExpirationDate == null) ?
                TrialStartDateStore.GetTrialStartDate().AddDays(14) : CurrentLicense.ExpirationDate.Value;

            var remainingDays = (expiration - now).Days;

            return remainingDays > 0 ? remainingDays : 0;
        }

        public bool IsLicenseExpired() => LicenseExpirationChecker.HasLicenseExpired(CurrentLicense);
    }
}