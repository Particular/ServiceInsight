namespace ServiceInsight.Tests
{
    using NUnit.Framework;
    using ServiceInsight.Shell;
    using Shouldly;

    [TestFixture]
    public class StatusBarManagerTests
    {
        private StatusBarManager statusBarManager;

        [SetUp]
        public void TestInitialize()
        {
            statusBarManager = new StatusBarManager();
        }

        [Test]
        public void Should_show_warning_ten_days_before_trial_gets_expired()
        {
            const int DaysRemaining = 10;

            statusBarManager.LicenseStatus.SetTrialRemainingDays(DaysRemaining);

            statusBarManager.LicenseStatus.ShowLicenseWarn.ShouldBe(true);
            statusBarManager.LicenseStatus.ShowLicenseError.ShouldBe(false);
        }

        [Test]
        public void Should_show_error_after_trial_license_gets_expired()
        {
            const int DaysRemaining = 0;

            statusBarManager.LicenseStatus.SetTrialRemainingDays(DaysRemaining);

            statusBarManager.LicenseStatus.ShowLicenseWarn.ShouldBe(false);
            statusBarManager.LicenseStatus.ShowLicenseError.ShouldBe(true);
        }

        [Test]
        public void Should_show_warning_ten_days_before_license_gets_expired()
        {
            const int DaysRemaining = 10;

            statusBarManager.LicenseStatus.SetLicenseRemainingDays(DaysRemaining);

            statusBarManager.LicenseStatus.ShowLicenseWarn.ShouldBe(true);
            statusBarManager.LicenseStatus.ShowLicenseError.ShouldBe(false);
        }

        [Test]
        public void Should_show_error_after_license_gets_expired()
        {
            const int DaysRemaining = 0;

            statusBarManager.LicenseStatus.SetLicenseRemainingDays(DaysRemaining);

            statusBarManager.LicenseStatus.ShowLicenseWarn.ShouldBe(false);
            statusBarManager.LicenseStatus.ShowLicenseError.ShouldBe(true);
        }

        [Test]
        public void Should_show_warning_ten_days_before_upgrade_protection_gets_expired()
        {
            const int DaysRemaining = 10;

            statusBarManager.LicenseStatus.SetLicenseUpgradeProtectionDays(DaysRemaining);

            statusBarManager.LicenseStatus.ShowLicenseWarn.ShouldBe(true);
            statusBarManager.LicenseStatus.ShowLicenseError.ShouldBe(false);
        }

        [Test]
        public void Should_show_warning_after_upgrade_protection_gets_expired()
        {
            const int DaysRemaining = 0;

            statusBarManager.LicenseStatus.SetLicenseUpgradeProtectionDays(DaysRemaining);

            statusBarManager.LicenseStatus.ShowLicenseWarn.ShouldBe(true);
            statusBarManager.LicenseStatus.ShowLicenseError.ShouldBe(false);
        }
    }
}