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

            statusBarManager.SetTrialRemainingDays(DaysRemaining);

            statusBarManager.ShowLicenseWarn.ShouldBe(true);
            statusBarManager.ShowLicenseError.ShouldBe(false);
        }

        [Test]
        public void Should_show_error_after_trial_license_gets_expired()
        {
            const int DaysRemaining = 0;

            statusBarManager.SetTrialRemainingDays(DaysRemaining);

            statusBarManager.ShowLicenseWarn.ShouldBe(false);
            statusBarManager.ShowLicenseError.ShouldBe(true);
        }

        [Test]
        public void Should_show_warning_ten_days_before_license_gets_expired()
        {
            const int DaysRemaining = 10;

            statusBarManager.SetLicenseRemainingDays(DaysRemaining);

            statusBarManager.ShowLicenseWarn.ShouldBe(true);
            statusBarManager.ShowLicenseError.ShouldBe(false);
        }

        [Test]
        public void Should_show_error_after_license_gets_expired()
        {
            const int DaysRemaining = 0;

            statusBarManager.SetLicenseRemainingDays(DaysRemaining);

            statusBarManager.ShowLicenseWarn.ShouldBe(false);
            statusBarManager.ShowLicenseError.ShouldBe(true);
        }

        [Test]
        public void Should_show_warning_ten_days_before_upgrade_protection_gets_expired()
        {
            const int DaysRemaining = 10;

            statusBarManager.SetLicenseUpgradeProtectionDays(DaysRemaining);

            statusBarManager.ShowLicenseWarn.ShouldBe(true);
            statusBarManager.ShowLicenseError.ShouldBe(false);
        }

        [Test]
        public void Should_show_warning_after_upgrade_protection_gets_expired()
        {
            const int DaysRemaining = 0;

            statusBarManager.SetLicenseUpgradeProtectionDays(DaysRemaining);

            statusBarManager.ShowLicenseWarn.ShouldBe(true);
            statusBarManager.ShowLicenseError.ShouldBe(false);
        }
    }
}