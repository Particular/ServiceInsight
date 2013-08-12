using System.Threading.Tasks;
using Autofac;
using Caliburn.PresentationFramework.Screens;
using Machine.Specifications;
using NSubstitute;
using Particular.ServiceInsight.Desktop.Shell;
using Particular.ServiceInsight.Tests.Helpers;

namespace NServiceBus.Profiler.Tests.Shell.Dialog
{
    [Subject("endpoint connection dialog")]
    public abstract class with_a_endpoint_connection_dialog
    {
        protected static IManagementService ManagementService;
        protected static IShellViewModel Shell;
        protected static ISettingsProvider SettingsProvider;
        protected static IContainer Container;
        protected static ILifetimeScope Scope;
        protected static IManagementConnectionProvider Connection;
        protected static ProfilerSettings StoredSetting;
        protected static ManagementConnectionViewModel ConnectTo;

        Establish context = () =>
        {
            Shell = Substitute.For<IShellViewModel>();
            ManagementService = Substitute.For<IManagementService>();
            SettingsProvider = Substitute.For<ISettingsProvider>();
            Connection = Substitute.For<IManagementConnectionProvider>();
            Container = RegisterContainer();
            StoredSetting = GetReloadedSettings();
            SettingsProvider.GetSettings<ProfilerSettings>().Returns(StoredSetting);
            ConnectTo = new ManagementConnectionViewModel(SettingsProvider, Container) { Parent = Shell };
        };

        private static IContainer RegisterContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(ManagementService);
            builder.RegisterInstance(Connection);

            return builder.Build();
        }

        private static ProfilerSettings GetReloadedSettings()
        {
            var settings = new ProfilerSettings();

            settings.RecentManagementApiEntries.Add("http://localhost/api");
            settings.RecentManagementApiEntries.Add("http://othermachine:8888/api");

            return settings;
        }
    }

    public class with_connection_to_management_api : with_a_endpoint_connection_dialog
    {
        Establish context = () => ManagementService.GetVersion().Returns(Task.Run(() => "1.0"));

        Because of = () =>
        {
            ((IActivate)ConnectTo).Activate();
            ConnectTo.ServiceUrl = "http://localhost:8080/managemnetApi";
            //ConnectTo.SetPrivate(x => x.Version, new VersionInfo {Version = "1.0"});
            AsyncHelper.Run(() => ConnectTo.Accept());
        };

        It should_allow_selecting_the_valid_service_url = () => ConnectTo.CanAccept().ShouldBeTrue();
        It should_consider_the_address_valid = () => ConnectTo.IsAddressValid.ShouldBeTrue();
        It should_store_connection_address = () => SettingsProvider.Received().SaveSettings(Arg.Any<ProfilerSettings>());
        It should_update_recent_connection_entries = () => StoredSetting.RecentManagementApiEntries.Count.ShouldEqual(3);
        It recent_connection_should_contain_last_connected_entry = () => StoredSetting.RecentManagementApiEntries.ShouldContain("http://localhost:8080/managemnetApi");
    }
}