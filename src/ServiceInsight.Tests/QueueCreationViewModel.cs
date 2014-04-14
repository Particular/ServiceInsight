using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.Explorer.QueueExplorer;
using NServiceBus.Profiler.Desktop.Models;
using NServiceBus.Profiler.Tests.Helpers;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace NServiceBus.Profiler.Tests
{
    [TestFixture]
    public class QueueCreationViewModel
    {
        private Desktop.Shell.QueueCreationViewModel Model;
        private IQueueManagerAsync QueueManager;
        private IQueueExplorerViewModel Explorer;
        private INetworkOperations Network;

        [SetUp]
        public void TestInitialize()
        {
            IList<string> machines = new List<string> { Environment.MachineName, "AnotherMachine" };
            QueueManager = Substitute.For<IQueueManagerAsync>();
            Explorer = Substitute.For<IQueueExplorerViewModel>();
            Network = Substitute.For<INetworkOperations>();
            Model = new Desktop.Shell.QueueCreationViewModel(QueueManager, Explorer, Network);
            Network.GetMachines().Returns(Task.FromResult(machines));
        }

        [Test]
        public void should_select_correct_default_values_when_displayed()
        {
            Model.IsTransactional.ShouldBe(true);
            Model.SelectedMachine.ShouldBe(null);
        }

        [Test]
        public void should_display_network_machine_names_when_retrieval_is_finished()
        {
            AsyncHelper.Run(() => ((IActivate)Model).Activate());

            Model.Machines.Count.ShouldBe(2);
            Model.WorkInProgress.ShouldBe(false);
        }

        [Test]
        public void should_allow_creating_a_queue()
        {
            Model.SelectedMachine = Environment.MachineName;
            Model.QueueName = "TestQueue";

            Model.CanAccept().ShouldBe(true);
        }

        [Test]
        public void should_request_the_queue_to_be_created_when_dialog_is_closed()
        {
            const string queueName = "TestQueue";
            var parent = new TestConductorScreen();
            var addedQueue = new Queue(Environment.MachineName, queueName);
            QueueManager.CreatePrivateQueue(Arg.Any<Queue>(), true).ReturnsForAnyArgs(addedQueue);

            Model.Parent = parent;
            Model.SelectedMachine = Environment.MachineName;
            Model.QueueName = queueName;
            Model.IsTransactional = true;
            Model.Accept();

            QueueManager.Received().CreatePrivateQueueAsync(Arg.Is<Queue>(q => q.Address.Equals("testqueue@.")), Arg.Is(true));
            Model.IsActive.ShouldBe(false);
        }
    }
}