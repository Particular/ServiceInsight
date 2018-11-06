namespace ServiceInsight.Framework.Licensing
{
    using Particular.Licensing;

    internal class LicenseDialogSource : LicenseSource
    {
        private string licenseText;

        private LicenseDialogSource(string licenseText)
            : base("license dialog")
        {
            this.licenseText = licenseText;
        }

        public static LicenseSourceResult Validate(string licenseText)
        {
            var source = new LicenseDialogSource(licenseText);
            var sourceResult = source.Find("ServiceInsight");

            if (sourceResult.License != null && LicenseExpirationChecker.HasLicenseExpired(sourceResult.License))
            {
                sourceResult.Result = "The selected license is expired";
            }

            return sourceResult;
        }

        public override LicenseSourceResult Find(string applicationName)
        {
            return ValidateLicense(licenseText, applicationName);
        }
    }
}