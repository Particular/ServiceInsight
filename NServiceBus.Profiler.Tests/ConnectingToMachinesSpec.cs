using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.PresentationFramework.Screens;
using Machine.Specifications;
using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.Shell;
using NServiceBus.Profiler.Tests.Helpers;
using NSubstitute;

namespace NServiceBus.Profiler.Tests.Shell.Dialog
{
    [Subject("connection dialog")]
    public abstract class with_a_connection_dialog
    {
        protected static ConnectToMachineViewModel ConnectTo;
        protected static INetworkOperations NetworkOperations;
        protected static TestConductorScreen Conductor;
        
        Establish context = () =>
        {
            IList<string> networkMachines = new List<string> {"FirstServer", "SecondServer", Environment.MachineName};
            NetworkOperations = Substitute.For<INetworkOperations>();
            NetworkOperations.GetMachines().Returns(Task.FromResult(networkMachines));
            Conductor = new TestConductorScreen();
            ConnectTo = new ConnectToMachineViewModel(NetworkOperations) { Parent = Conductor };
        };
    }

    public class with_an_activated_dialog : with_a_connection_dialog
    {
        Because of = () =>
        {
             AsyncHelper.Run(() => ((IScreen) ConnectTo).Activate());
        };

        It should_have_list_of_machines_on_the_network_prepopulated = () => ConnectTo.Machines.Count.ShouldBeGreaterThan(0);
        It should_have_my_machine_name_in_the_list = () => ConnectTo.Machines.Contains(Environment.MachineName).ShouldBeTrue();
    }

    public class with_an_invalid_selection : with_a_connection_dialog
    {
        Establish context = () =>
        {
            ((IScreen) ConnectTo).Activate();
            ConnectTo.ComputerName = "NonExistingMachine";
        };

        Because of = () => ConnectTo.Accept();

        It should_allow_accepting_the_dialog = () => ConnectTo.CanAccept().ShouldBeTrue();
        It should_not_allow_closing_the_dialog = () => ConnectTo.IsActive.ShouldBeTrue();
    }

    public class with_an_invalid_selection_cancelling_the_dialog : with_a_connection_dialog
    {
        Establish context = () =>
        {
            ((IScreen)ConnectTo).Activate();
            ConnectTo.ComputerName = "NonExistingMachine";
        };

        Because of = () => ConnectTo.Close();

        It should_allow_cancelling_the_dialog = () => ConnectTo.IsActive.ShouldBeFalse();
    }

    public class with_a_valid_computer_name_selected : with_a_connection_dialog
    {
        Establish context = () =>
        {
            ((IScreen) ConnectTo).Activate();
            ConnectTo.ComputerName = Environment.MachineName;
        };

        Because of = () => ConnectTo.Accept();

        It checks_if_a_valid_address_is_selected = () => ConnectTo.CanAccept().ShouldBeTrue();
    }
}