namespace Particular.ServiceInsight.Tests
{
    using Autofac;
    using Caliburn.PresentationFramework.Screens;
    using Desktop.Core.Settings;
    using Desktop.ServiceControl;
    using Desktop.Settings;
    using Desktop.Shell;
    using Helpers;
    using NSubstitute;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class ServiceControlConnectionDialogTests
    {
        DefaultServiceControl serviceControl;
        ShellViewModel shell;
        ISettingsProvider settingsProvider;
        IContainer container;
        IServiceControlConnectionProvider connection;
        ProfilerSettings storedSetting;
        ServiceControlConnectionViewModel connectTo;

        [SetUp]
        public void TestInitialize()
        {
            shell = Substitute.For<ShellViewModel>();
            serviceControl = Substitute.For<DefaultServiceControl>();
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
            storedSetting.RecentServiceControlEntries.Count.ShouldBe(3);
            storedSetting.RecentServiceControlEntries.ShouldContain("http://localhost:8080/managemnetApi");
        }

        ProfilerSettings GetReloadedSettings()
        {
            var settings = new ProfilerSettings();

            settings.RecentServiceControlEntries.Add("http://localhost/api");
            settings.RecentServiceControlEntries.Add("http://othermachine:8888/api");

            return settings;
        }

        IContainer RegisterContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(serviceControl);
            builder.RegisterInstance(connection);

            return builder.Build();
        }

    }
}