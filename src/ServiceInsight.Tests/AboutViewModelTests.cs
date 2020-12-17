namespace ServiceInsight.Tests
{
    using Caliburn.Micro;
    using NSubstitute;
    using NUnit.Framework;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.Licensing;
    using ServiceControl;
    using Shell;
    using Shouldly;
    using System.Threading.Tasks;

    [TestFixture]
    public class AboutViewModelTests
    {
        NetworkOperations networkOperations;
        IServiceControl serviceControl;
        AppLicenseManager licenseManager;
        IApplicationVersionService versionService;
        AboutViewModel sut;
        ServiceControlClientRegistry clientRegistry;

        [SetUp]
        public void Initialize()
        {
            networkOperations = Substitute.For<NetworkOperations>();
            serviceControl = Substitute.For<IServiceControl>();
            versionService = Substitute.For<IApplicationVersionService>();
            licenseManager = Substitute.For<AppLicenseManager>();
            clientRegistry = Substitute.For<ServiceControlClientRegistry>();

            clientRegistry.GetServiceControl(Arg.Any<string>()).Returns(serviceControl);
            
            sut = new AboutViewModel(networkOperations, versionService, licenseManager, clientRegistry);
        }

        [Test]
        public async Task Should_fetch_service_control_version()
        {
            ((IActivate)sut).Activate();

            await clientRegistry.Received(1).GetVersions();
        }

        [Test]
        public void Should_display_multi_connection_when_more_than_one_service_control_version()
        {
            var versions = new[] {"3.0.0", "4.0.0"};
            clientRegistry.GetVersions().Returns(versions);

            ((IActivate)sut).Activate();

            sut.ServiceControlVersion.ShouldBe(AboutViewModel.MultipleConnection);
        }
        
        [Test]
        public void Should_display_service_control_version()
        {
            const string ServiceControlVersion = "0.8.0-Unstable379";
            clientRegistry.GetVersions().Returns(new[] { ServiceControlVersion });

            ((IActivate)sut).Activate();

            sut.ServiceControlVersion.ShouldBe(ServiceControlVersion);
        }

        [Test]
        public void Should_display_application_version_number()
        {
            const string ApplicationVersion = "0.8.0-Unstable379";
            versionService.GetVersion().Returns(ApplicationVersion);

            ((IActivate)sut).Activate();

            sut.AppVersion.ShouldNotBe(null);
            sut.AppVersion.ShouldBe(ApplicationVersion);
        }

        [Test]
        public void Should_display_short_commit_hash()
        {
            const string ApplicationVersion = "f1b04543"; //random hash
            versionService.GetCommitHash().Returns(ApplicationVersion);

            ((IActivate)sut).Activate();

            sut.CommitHash.ShouldNotBe(null);
            sut.CommitHash.ShouldBe(ApplicationVersion);
        }
    }
}