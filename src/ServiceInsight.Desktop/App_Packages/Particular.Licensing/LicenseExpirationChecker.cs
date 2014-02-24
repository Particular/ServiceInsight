namespace Particular.Licensing
{
    using System;

    static class LicenseExpirationChecker
    {
        public static bool HasLicenseExpired(License license, out string expirationReason)
        {
            if (license.ExpirationDate.HasValue && HasLicenseDateExpired(license.ExpirationDate.Value))
            {
                expirationReason = "Your license has expired.";
                return true;
            }


            if (license.UpgradeProtectionExpiration != null)
            {
                var buildTimeStamp = ReleaseDateReader.GetReleaseDate();
                if (buildTimeStamp > license.UpgradeProtectionExpiration)
                {
                    expirationReason = "Your upgrade protection does not cover this version of NServiceBus.";
                    return true;
                }
            }
            expirationReason = null;
            return false;
        }

        static bool HasLicenseDateExpired(DateTime licenseDate)
        {
            var oneDayGrace = licenseDate >= DateTime.MaxValue.Date ? licenseDate : licenseDate.AddDays(1);
            return oneDayGrace < DateTime.UtcNow.Date;
        }
    }
}