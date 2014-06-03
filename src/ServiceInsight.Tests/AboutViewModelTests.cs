namespace Particular.ServiceInsight.Tests
{
    using Caliburn.Micro;
    using Desktop.Core;
    using Desktop.ServiceControl;
    using Desktop.Shell;
    using NSubstitute;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class AboutViewModelTests
    {
        NetworkOperations networkOperations;
        DefaultServiceControl serviceControl;
        LicenseRegistrationViewModel licenseRegistration;
        AboutViewModel sut;

        [SetUp]
        public void Initialize()
        {
            networkOperations = Substitute.For<NetworkOperations>();
            serviceControl = Substitute.For<DefaultServiceControl>();
            licenseRegistration = Substitute.For<LicenseRegistrationViewModel>();

            sut = new AboutViewModel(networkOperations, serviceControl, licenseRegistration);
        }

        [Test]
        public void Should_fetch_service_control_version()
        {
            ((IActivate)sut).Activate();

            serviceControl.Received(1).GetVersion();
        }

        [Test]
        public void Should_display_service_control_version()
        {
            const string ServiceControlVersion = "0.8.0-Unstable379";
            serviceControl.GetVersion().Returns(ServiceControlVersion);

            ((IActivate)sut).Activate();

            sut.ServiceControlVersion.ShouldBe(ServiceControlVersion);
        }

        [Test]
        public void Should_display_application_version_number()
        {
            ((IActivate)sut).Activate();

            sut.AppVersion.ShouldNotBe(null);
            sut.AppVersion.ShouldNotBeEmpty();
        }

        [Test]
        public void Should_display_short_commit_hash()
        {
            ((IActivate)sut).Activate();

            sut.CommitHash.ShouldNotBe(null);
            sut.CommitHash.Length.ShouldBe(7);
        }
    }
}