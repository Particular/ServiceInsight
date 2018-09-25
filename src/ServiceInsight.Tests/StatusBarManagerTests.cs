namespace ServiceInsight.Tests
{
    using NSubstitute;
    using NUnit.Framework;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.UI.ScreenManager;
    using ServiceInsight.Shell;
    using Shouldly;

    [TestFixture]
    public class StatusBarManagerTests
    {
        private LicenseStatusBar licenseStatusBar;

        [SetUp]
        public void TestInitialize()
        {
            licenseStatusBar = new LicenseStatusBar(
                Substitute.For<IWindowManagerEx>(),
                Substitute.For<NetworkOperations>());
        }

        [Test]
        public void Should_show_warning_ten_days_before_trial_gets_expired()
        {
            const int DaysRemaining = 10;

            licenseStatusBar.SetTrialRemainingDays(DaysRemaining);

            licenseStatusBar.ShowLicenseWarn.ShouldBe(true);
            licenseStatusBar.ShowLicenseError.ShouldBe(false);
        }

        [Test]
        public void Should_show_error_after_trial_license_gets_expired()
        {
            const int DaysRemaining = 0;

            licenseStatusBar.SetTrialRemainingDays(DaysRemaining);

            licenseStatusBar.ShowLicenseWarn.ShouldBe(false);
            licenseStatusBar.ShowLicenseError.ShouldBe(true);
        }

        [Test]
        public void Should_show_warning_ten_days_before_license_gets_expired()
        {
            const int DaysRemaining = 10;

            licenseStatusBar.SetLicenseRemainingDays(DaysRemaining);

            licenseStatusBar.ShowLicenseWarn.ShouldBe(true);
            licenseStatusBar.ShowLicenseError.ShouldBe(false);
        }

        [Test]
        public void Should_show_error_after_license_gets_expired()
        {
            const int DaysRemaining = 0;

            licenseStatusBar.SetLicenseRemainingDays(DaysRemaining);

            licenseStatusBar.ShowLicenseWarn.ShouldBe(false);
            licenseStatusBar.ShowLicenseError.ShouldBe(true);
        }

        [Test]
        public void Should_show_warning_ten_days_before_upgrade_protection_gets_expired()
        {
            const int DaysRemaining = 10;

            licenseStatusBar.SetLicenseUpgradeProtectionDays(DaysRemaining);

            licenseStatusBar.ShowLicenseWarn.ShouldBe(true);
            licenseStatusBar.ShowLicenseError.ShouldBe(false);
        }

        [Test]
        public void Should_show_warning_after_upgrade_protection_gets_expired()
        {
            const int DaysRemaining = 0;

            licenseStatusBar.SetLicenseUpgradeProtectionDays(DaysRemaining);

            licenseStatusBar.ShowLicenseWarn.ShouldBe(true);
            licenseStatusBar.ShowLicenseError.ShouldBe(false);
        }
    }
}