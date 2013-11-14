using Autofac;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Core.Settings;
using NServiceBus.Profiler.Desktop.ServiceControl;
using NServiceBus.Profiler.Desktop.Settings;
using NServiceBus.Profiler.Desktop.Shell;
using NServiceBus.Profiler.Tests.Helpers;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace NServiceBus.Profiler.Tests
{
    [TestFixture]
    public class ServiceControlConnectionDialogTests
    {
        private IServiceControl serviceControl;
        private IShellViewModel shell;
        private ISettingsProvider settingsProvider;
        private IContainer container;
        private IServiceControlConnectionProvider connection;
        private ProfilerSettings storedSetting;
        private ServiceControlConnectionViewModel connectTo;

        [SetUp]
        public void TestInitialize()
        {
            shell = Substitute.For<IShellViewModel>();
            serviceControl = Substitute.For<IServiceControl>();
            settingsProvider = Substitute.For<ISettingsProvider>();
            connection = Substitute.For<IServiceControlConnectionProvider>();
            container = RegisterContainer();
            storedSetting = GetReloadedSettings();
            settingsProvider.GetSettings<ProfilerSettings>().Returns(storedSetting);
            connectTo = new ServiceControlConnectionViewModel(settingsProvider, container) { Parent = shell }; //TODO: Do we need to pass the full container here?
        }

        [Test]
        public void should_be_able_to_connect_to_a_valid_service()
        {
            ((IActivate)connectTo).Activate();
            connectTo.ServiceUrl = "http://localhost:8080/managemnetApi";
            AsyncHelper.Run(() => connectTo.Accept());

            connectTo.CanAccept().ShouldBe(true);
            connectTo.IsAddressValid.ShouldBe(true);
        }

        [Test]
        public void should_store_connection_address_and_add_it_to_recent_entries()
        {
            ((IActivate)connectTo).Activate();
            connectTo.ServiceUrl = "http://localhost:8080/managemnetApi";
            AsyncHelper.Run(() => connectTo.Accept());

            settingsProvider.Received().SaveSettings(Arg.Any<ProfilerSettings>());
            storedSetting.RecentManagementApiEntries.Count.ShouldBe(3);
            storedSetting.RecentManagementApiEntries.ShouldContain("http://localhost:8080/managemnetApi");
        }

        private ProfilerSettings GetReloadedSettings()
        {
            var settings = new ProfilerSettings();

            settings.RecentManagementApiEntries.Add("http://localhost/api");
            settings.RecentManagementApiEntries.Add("http://othermachine:8888/api");

            return settings;
        }

        private IContainer RegisterContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(serviceControl);
            builder.RegisterInstance(connection);

            return builder.Build();
        }

    }
}