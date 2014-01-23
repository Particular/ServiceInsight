using System.Threading.Tasks;
using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.ServiceControl;
using NServiceBus.Profiler.Desktop.Shell;
using NServiceBus.Profiler.Tests.Helpers;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace NServiceBus.Profiler.Tests
{
    [TestFixture]
    public class AboutViewModelTests
    {
        private INetworkOperations _networkOperations;
        private IServiceControl _serviceControl;
        private ILicenseRegistrationViewModel _licenseRegistration;
        private AboutViewModel _sut;

        [SetUp]
        public void Initialize()
        {
            _networkOperations = Substitute.For<INetworkOperations>();
            _serviceControl = Substitute.For<IServiceControl>();
            _licenseRegistration = Substitute.For<ILicenseRegistrationViewModel>();

            _sut = new AboutViewModel(_networkOperations, _serviceControl, _licenseRegistration);
        }

        [Test]
        public void Should_fetch_service_control_version()
        {
            AsyncHelper.Run(() => _sut.Activate());

            _serviceControl.Received(1).GetVersion();
        }

        [Test]
        public void Should_display_service_control_version()
        {
            const string ServiceControlVersion = "0.8.0-Unstable379";
            _serviceControl.GetVersion().Returns(Task.FromResult(ServiceControlVersion));

            AsyncHelper.Run(() => _sut.Activate());

            _sut.ServiceControlVersion.ShouldBe(ServiceControlVersion);
        }

        [Test]
        public void Should_display_application_version_number()
        {
            AsyncHelper.Run(() => _sut.Activate());

            _sut.AppVersion.ShouldNotBe(null);
            _sut.AppVersion.ShouldNotBeEmpty();
        }

        [Test]
        public void Should_display_short_commit_hash()
        {
            AsyncHelper.Run(() => _sut.Activate());

            _sut.CommitHash.ShouldNotBe(null);
            _sut.CommitHash.Length.ShouldBe(7);
        }
    }
}