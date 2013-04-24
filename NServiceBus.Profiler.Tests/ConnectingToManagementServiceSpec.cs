using System.Threading.Tasks;
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
            StoredSetting = new ProfilerSettings();
            SettingsProvider.GetSettings<ProfilerSettings>().Returns(StoredSetting);
            ConnectTo = new ManagementConnectionViewModel(ManagementService, SettingsProvider) { Parent = Shell };
        };
    }

    public class with_connection_to_management_api : with_a_endpoint_connection_dialog
    {
        Establish context = () =>
        {
            ManagementService.IsAlive(Arg.Any<string>()).Returns(Task.Run(() => true));
        };

        Because of = () =>
        {
            ConnectTo.ServiceUrl = "http://localhost:8080/managemnetApi";
            AsyncHelper.Run(() => ConnectTo.Accept());
        };

        It should_consider_the_address_valid = () => ConnectTo.IsAddressValid.ShouldBeTrue();
        It should_store_connection_address = () => SettingsProvider.Received().SaveSettings(Arg.Any<ProfilerSettings>());
    }
}