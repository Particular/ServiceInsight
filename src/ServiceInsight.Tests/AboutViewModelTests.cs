namespace ServiceInsight.Tests
{
    using Caliburn.Micro;
    using NSubstitute;
    using NUnit.Framework;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.Licensing;
    using ServiceInsight.ServiceControl;
    using ServiceInsight.Shell;
    using Shouldly;

    [TestFixture]
    public class AboutViewModelTests
    {
        NetworkOperations networkOperations;
        IServiceControl serviceControl;
        AppLicenseManager licenseManager;
        IApplicationVersionService versionService;
        LicenseRegistrationViewModel licenseRegistration;
        AboutViewModel sut;

        [SetUp]
        public void Initialize()
        {
            networkOperations = Substitute.For<NetworkOperations>();
            serviceControl = Substitute.For<IServiceControl>();
            licenseRegistration = Substitute.For<LicenseRegistrationViewModel>();
            versionService = Substitute.For<IApplicationVersionService>();

            sut = new AboutViewModel(networkOperations, versionService, serviceControl, licenseRegistration);
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