﻿namespace ServiceInsight.Tests
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
        LicenseStatusBar licenseStatusBar;

        [SetUp]
        public void TestInitialize()
        {
            licenseStatusBar = new LicenseStatusBar(
                Substitute.For<IServiceInsightWindowManager>(),
                Substitute.For<NetworkOperations>());
        }

        [Test]
        public void Should_show_warning_ten_days_before_trial_gets_expired()
        {
            const int DaysRemaining = 10;

            licenseStatusBar.SetNonProductionRemainingDays(DaysRemaining);

            licenseStatusBar.ShowLicenseWarn.ShouldBe(true);
            licenseStatusBar.ShowLicenseError.ShouldBe(false);
        }

        [Test]
        public void Should_show_error_after_trial_license_gets_expired()
        {
            const int DaysRemaining = 0;

            licenseStatusBar.SetNonProductionRemainingDays(DaysRemaining);

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
        public void Should_a_warn_when_the_license_expires_today()
        {
            const int DaysRemaining = 0;

            licenseStatusBar.SetLicenseRemainingDays(DaysRemaining);

            licenseStatusBar.ShowLicenseWarn.ShouldBe(true);
            licenseStatusBar.ShowLicenseError.ShouldBe(false);
        }

        [Test]
        public void Should_show_error_after_license_gets_expired()
        {
            const int DaysRemaining = -1;

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