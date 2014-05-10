namespace Particular.ServiceInsight.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Caliburn.PresentationFramework.Screens;
    using Desktop.Core;
    using Desktop.Shell;
    using Helpers;
    using NSubstitute;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class QueueConnectionDialogTests
    {
        private ConnectToMachineViewModel ConnectTo;
        private INetworkOperations NetworkOperations;
        private TestConductorScreen Conductor;

        [SetUp]
        public void TestInitialize()
        {
            IList<string> networkMachines = new List<string> { "FirstServer", "SecondServer", Environment.MachineName };
            NetworkOperations = Substitute.For<INetworkOperations>();
            NetworkOperations.GetMachines().Returns(Task.FromResult(networkMachines));
            Conductor = new TestConductorScreen();
            ConnectTo = new ConnectToMachineViewModel(NetworkOperations) { Parent = Conductor };
        }

        [Test]
        public void should_see_network_machine_names()
        {
            AsyncHelper.Run((() => ((IScreen) ConnectTo).Activate()));

            ConnectTo.Machines.Count.ShouldBeGreaterThan(0);
            ConnectTo.Machines.Contains(Environment.MachineName).ShouldBe(true);
        }

        [Test]
        public void should_not_allow_accepting_the_dialog_when_invalid_selection_is_made()
        {
            AsyncHelper.Run(() => ((IScreen) ConnectTo).Activate());

            ConnectTo.ComputerName = "NonExistingMachine";
            ConnectTo.Accept();

            ConnectTo.CanAccept().ShouldBe(true);
            ConnectTo.IsActive.ShouldBe(true);
        }

        [Test]
        public void should_allow_cancelling_the_dialog_when_invalid_selection_is_made()
        {
            AsyncHelper.Run(() => ((IScreen) ConnectTo).Activate());

            ConnectTo.ComputerName = "NonExistingMachine";
            ConnectTo.Close();

            ConnectTo.CanAccept().ShouldBe(true);
            ConnectTo.IsActive.ShouldBe(false);
        }

        [Test]
        public void should_allow_accepting_the_dialog_when_valid_selection_is_made()
        {
            AsyncHelper.Run(() => ((IScreen)ConnectTo).Activate());

            ConnectTo.ComputerName = Environment.MachineName;
            
            ConnectTo.CanAccept().ShouldBe(true);
        }
    }
}