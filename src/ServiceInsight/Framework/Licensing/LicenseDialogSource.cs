namespace ServiceInsight.Framework.Licensing
{
    using Particular.Licensing;

    class LicenseDialogSource : LicenseSource
    {
        string licenseText;

        LicenseDialogSource(string licenseText)
            : base("license dialog")
        {
            this.licenseText = licenseText;
        }

        public static LicenseSourceResult Validate(string licenseText)
        {
            var source = new LicenseDialogSource(licenseText);
            var sourceResult = source.Find("ServiceInsight");

            if (sourceResult.License != null && sourceResult.License.HasExpired())
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