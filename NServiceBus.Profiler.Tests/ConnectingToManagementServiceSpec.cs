using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.PresentationFramework.Screens;
using Machine.Specifications;
using NServiceBus.Profiler.Common.Settings;
using NServiceBus.Profiler.Core.Management;
using NServiceBus.Profiler.Core.Settings;
using NServiceBus.Profiler.Desktop.Shell;
using NServiceBus.Profiler.Tests.Helpers;
using NSubstitute;

namespace NServiceBus.Profiler.Tests.Shell.Dialog
{
    [Subject("endpoint connection dialog")]
    public abstract class with_a_endpoint_connection_dialog
    {
        protected static IManagementService ManagementService;
        protected static IShellViewModel Shell;
        protected static ISettingsProvider SettingsProvider;
        protected static ProfilerSettings StoredSetting;
        protected static ManagementConnectionViewModel ConnectTo;
        
        Establish context = () =>
        {
            Shell = Substitute.For<IShellViewModel>();
            ManagementService = Substitute.For<IManagementService>();
            SettingsProvider = Substitute.For<ISettingsProvider>();
            StoredSetting = GetReloadedSettings();
            SettingsProvider.GetSettings<ProfilerSettings>().Returns(StoredSetting);
            ConnectTo = new ManagementConnectionViewModel(ManagementService, SettingsProvider) { Parent = Shell };
        };

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
        Establish context = () => ManagementService.IsAlive(Arg.Any<string>()).Returns(Task.Run(() => true));

        Because of = () =>
        {
            ((IActivate)ConnectTo).Activate();
            ConnectTo.ServiceUrl = "http://localhost:8080/managemnetApi";
            AsyncHelper.Run(() => ConnectTo.Accept());
        };

        It should_allow_selecting_the_valid_service_url = () => ConnectTo.CanAccept().ShouldBeTrue();
        It should_consider_the_address_valid = () => ConnectTo.IsAddressValid.ShouldBeTrue();
        It should_store_connection_address = () => SettingsProvider.Received().SaveSettings(Arg.Any<ProfilerSettings>());
        It should_update_recent_connection_entries = () => StoredSetting.RecentManagementApiEntries.Count.ShouldEqual(3);
        It recent_connection_should_contain_last_connected_entry = () => StoredSetting.RecentManagementApiEntries.ShouldContain("http://localhost:8080/managemnetApi");
    }
}