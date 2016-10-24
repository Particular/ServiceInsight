namespace ServiceInsight.Tests
{
    using NSubstitute;
    using NUnit.Framework;
    using ServiceInsight.Framework;
    using ServiceInsight.ServiceControl;
    using ServiceInsight.Shell;
    using Shouldly;

    [TestFixture]
    public class AboutViewModelTests
    {
        NetworkOperations networkOperations;
        IServiceControl serviceControl;
        LicenseRegistrationViewModel licenseRegistration;

        [SetUp]
        public void Initialize()
        {
            networkOperations = Substitute.For<NetworkOperations>();
            serviceControl = Substitute.For<IServiceControl>();
            licenseRegistration = Substitute.For<LicenseRegistrationViewModel>();
        }

        [Test]
        public void Should_fetch_service_control_version()
        {
            var sut = CreateViewModel();

            serviceControl.Received(1).GetVersion();
        }

        [Test]
        public void Should_display_service_control_version()
        {
            const string ServiceControlVersion = "0.8.0-Unstable379";
            serviceControl.GetVersion().Returns(ServiceControlVersion);

            var sut = CreateViewModel();

            sut.ServiceControlVersion.ShouldBe(ServiceControlVersion);
        }

        [Test]
        public void Should_display_application_version_number()
        {
            var sut = CreateViewModel();

            sut.AppVersion.ShouldNotBe(null);
            sut.AppVersion.ShouldNotBeEmpty();
        }

        [Test]
        public void Should_display_short_commit_hash()
        {
            var sut = CreateViewModel();

            sut.CommitHash.ShouldNotBe(null);
            sut.CommitHash.Length.ShouldBe(7);
        }

        private AboutViewModel CreateViewModel()
        {
            return new AboutViewModel(networkOperations, serviceControl, licenseRegistration);
        }
    }
}