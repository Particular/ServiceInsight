namespace Particular.Licensing
{
    using System;
    using System.Collections.Generic;

    class License
    {
        public License()
        {
            ValidApplications = new List<string>();
        }

        public DateTime? ExpirationDate { get; set; }

        public bool IsTrialLicense => !IsCommercialLicense;

        public bool IsExtendedTrial { get; set; }

        public bool IsCommercialLicense
        {
            get
            {
                // i.e. No license file
                if (LicenseType == null)
                {
                    return false;
                }

                // Pre-2020 trial licenses were "Trial"
                if (LicenseType.Equals("trial", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                // Development licenses are currently "Non-Production Development"
                if (LicenseType.IndexOf("non-production", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return false;
                }

                // All other license types are commercial licenses
                return true;
            }
        }

        public string LicenseType { get; set; }

        public string Edition { get; set; }

        public string RegisteredTo { get; set; }

        public DateTime? UpgradeProtectionExpiration { get; internal set; }

        public List<string> ValidApplications { get; internal set; }

        public static License TrialLicense(DateTime trialStartDate)
        {
            return new License
            {
                LicenseType = "Trial",
                ExpirationDate = trialStartDate.AddDays(14),
                IsExtendedTrial = false,
                ValidApplications = new List<string> { "All" }
            };
        }

        public bool ValidForApplication(string applicationName)
        {
            return ValidApplications.Contains(applicationName) || ValidApplications.Contains("All") || applicationName == "NServiceBus";
        }

        /// <summary>
        /// Indicates whether or not the license has expired.
        /// </summary>
        public bool HasExpired()
        {
            switch (GetLicenseStatus())
            {
                case LicenseStatus.InvalidDueToExpiredTrial:
                case LicenseStatus.InvalidDueToExpiredSubscription:
                case LicenseStatus.InvalidDueToExpiredUpgradeProtection:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns the number of days until the license's <see cref="License.ExpirationDate" /> expires. Returns null if the license does not have an expiration date.
        /// </summary>
        public int? GetDaysUntilLicenseExpires()
        {
            if (ExpirationDate.HasValue)
            {
                var expiresIn = ExpirationDate.Value.Date - utcDateTimeProvider().Date;
                return expiresIn.Days;
            }

            return null;
        }

        /// <summary>
        /// Returns the number of days until the license's <see cref="License.UpgradeProtectionExpiration" /> expires. Returns null if the license does not have an ugprade protection expiration date.
        /// </summary>
        public int? GetDaysUntilUpgradeProtectionExpires()
        {
            if (UpgradeProtectionExpiration.HasValue)
            {
                var upgradeProtectionExpiresIn = UpgradeProtectionExpiration.Value.Date - utcDateTimeProvider().Date;
                return upgradeProtectionExpiresIn.Days;
            }

            return null;
        }

        /// <summary>
        /// Returns the current status of the license. The status indicates whether the license is
        /// valid at the moment and whether the license status is going to change soon.
        /// </summary>
        public LicenseStatus GetLicenseStatus()
        {
            var upgradeProtectionExpiresIn = GetDaysUntilUpgradeProtectionExpires();

            if (upgradeProtectionExpiresIn < 0)
            {
                // upgrade protection expired, check if this version is included in the upgrade protection:
                if (releaseDateProvider() > UpgradeProtectionExpiration)
                {
                    // this release is newer than allowed by the upgrade protection
                    return LicenseStatus.InvalidDueToExpiredUpgradeProtection;
                }

                // upgrade protection expired but is valid with this version as it has been released before the expiration
                return LicenseStatus.ValidWithExpiredUpgradeProtection;
            }

            var expiresIn = GetDaysUntilLicenseExpires();

            if (expiresIn < 0)
            {
                return IsTrialLicense ? LicenseStatus.InvalidDueToExpiredTrial : LicenseStatus.InvalidDueToExpiredSubscription;
            }

            if (IsTrialLicense)
            {
                return expiresIn <= trialExpirationWarningThresholdDays ? LicenseStatus.ValidWithExpiringTrial : LicenseStatus.Valid;
            }

            if (upgradeProtectionExpiresIn < (expiresIn ?? int.MaxValue))
            {
                return upgradeProtectionExpiresIn <= expirationWarningThresholdDays ? LicenseStatus.ValidWithExpiringUpgradeProtection : LicenseStatus.Valid;
            }

            return expiresIn <= expirationWarningThresholdDays ? LicenseStatus.ValidWithExpiringSubscription : LicenseStatus.Valid;
        }

        internal Func<DateTime> releaseDateProvider = () => ReleaseDateReader.GetReleaseDate().Date;

        internal Func<DateTime> utcDateTimeProvider = () => DateTime.UtcNow;

        const int expirationWarningThresholdDays = 10;
        const int trialExpirationWarningThresholdDays = 5;
    }
}