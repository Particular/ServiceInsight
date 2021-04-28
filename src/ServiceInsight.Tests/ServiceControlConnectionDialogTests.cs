namespace ServiceInsight.Tests
{
    using System.Threading.Tasks;
    using Autofac;
    using Caliburn.Micro;
    using NSubstitute;
    using NUnit.Framework;
    using ServiceInsight.Framework.Settings;
    using ServiceInsight.ServiceControl;
    using ServiceInsight.Settings;
    using ServiceInsight.Shell;
    using Shouldly;

    [TestFixture]
    public class ServiceControlConnectionDialogTests
    {
        ShellViewModel shell;
        ISettingsProvider settingsProvider;
        ServiceControlClientRegistry clientRegistry;
        ProfilerSettings storedSetting;
        ServiceControlConnectionViewModel connectTo;
        IServiceControl serviceControl;

        [SetUp]
        public void TestInitialize()
        {
            shell = Substitute.For<ShellViewModel>();
            settingsProvider = Substitute.For<ISettingsProvider>();
            serviceControl = Substitute.For<IServiceControl>();
            clientRegistry = Substitute.For<ServiceControlClientRegistry>();
            clientRegistry.Create(Arg.Any<string>()).Returns(serviceControl);
            storedSetting = GetReloadedSettings();
            settingsProvider.GetSettings<ProfilerSettings>().Returns(storedSetting);
            connectTo = new ServiceControlConnectionViewModel(settingsProvider, clientRegistry) { Parent = shell };
        }

        [Test]
        public async Task Should_be_able_to_connect_to_a_valid_service()
        {
            var url = "http://localhost:8080/managementApi";
            serviceControl.IsAlive().Returns((true, url));
            serviceControl.GetVersion().Returns((url, url));
            ((IActivate)connectTo).Activate();
            connectTo.ServiceUrl = url;
            await connectTo.Accept();

            connectTo.CanAccept().ShouldBe(true);
            connectTo.ShowError.ShouldBe(false);
        }

        [Test]
        public async Task Should_store_connection_address_and_add_it_to_recent_entries()
        {
            var url = "http://localhost:8080/managementApi";
            serviceControl.IsAlive().Returns((true, url));
            serviceControl.GetVersion().Returns((url, url));

            ((IActivate)connectTo).Activate();
            connectTo.ServiceUrl = url;
            await connectTo.Accept();

            settingsProvider.Received().SaveSettings(Arg.Any<ProfilerSettings>());
            storedSetting.RecentServiceControlEntries.Count.ShouldBe(3);
            storedSetting.RecentServiceControlEntries.ShouldContain(url);
        }

        ProfilerSettings GetReloadedSettings()
        {
            var settings = new ProfilerSettings();

            settings.RecentServiceControlEntries.Add("http://localhost/api");
            settings.RecentServiceControlEntries.Add("http://othermachine:8888/api");

            return settings;
        }

        // ILifetimeScope RegisterContainer()
        // {
        //     var builder = new ContainerBuilder();
        //
        //     builder.RegisterInstance(serviceControl);
        //     builder.RegisterInstance(connection);
        //
        //     return builder.Build();
        // }
    }
}