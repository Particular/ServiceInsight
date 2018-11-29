namespace Particular.Licensing
{
    enum LicenseStatus
    {
        Valid,

        // The Upgrade Protection already expired but the current version of the software is released before the UP expiration date.
        ValidWithExpiredUpgradeProtection,

        // The trial version is expiring soon.
        ValidWithExpiringTrial,

        // The subscription license is expiring soon.
        ValidWithExpiringSubscription,

        // The upgrade protection is expiring soon.
        ValidWithExpiringUpgradeProtection,

        // The trial version has expired.
        InvalidDueToExpiredTrial,

        // The subscription license has expired.
        InvalidDueToExpiredSubscription,

        // The current version of the software is not included in the license's Upgrade Protection.
        InvalidDueToExpiredUpgradeProtection
    }
}