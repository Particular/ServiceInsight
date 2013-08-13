using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.PresentationFramework.Screens;
using Machine.Specifications;
using NSubstitute;
using Particular.ServiceInsight.Desktop.Core;
using Particular.ServiceInsight.Desktop.Explorer.QueueExplorer;
using Particular.ServiceInsight.Desktop.Models;
using Particular.ServiceInsight.Desktop.Shell;
using Particular.ServiceInsight.Tests.Helpers;

namespace Particular.ServiceInsight.Tests.QueueCreation
{
    [Subject("queue creation")]
    public class with_queue_creation_view_model
    {
        protected static QueueCreationViewModel Model;
        protected static IQueueManager QueueManager;
        protected static IQueueExplorerViewModel Explorer;
        protected static INetworkOperations Network;

        Establish context = () =>
        {
            IList<string> machines = new List<string> {Environment.MachineName, "AnotherMachine"};
            QueueManager = Substitute.For<IQueueManager>();
            Explorer = Substitute.For<IQueueExplorerViewModel>();
            Network = Substitute.For<INetworkOperations>();
            Model = new QueueCreationViewModel(QueueManager, Explorer, Network);
            Network.GetMachines().Returns(Task.FromResult(machines));
        };

        It creates_transactional_queues_by_default = () => Model.IsTransactional.ShouldBeTrue();
        It has_not_selected_the_machine_name_by_default = () => Model.SelectedMachine.ShouldBeNull();
    }
    
    public class when_network_machines_are_fetched : with_queue_creation_view_model
    {
        Because of = () => AsyncHelper.Run(() => ((IActivate) Model).Activate());

        It has_populated_list_of_available_machines = () => Model.Machines.Count.ShouldEqual(2);
        It is_finished_fetching_network_machine_names = () => Model.WorkInProgress.ShouldBeFalse();
    }

    public class when_creating_a_queue : with_queue_creation_view_model
    {
        Because of = () =>
        {
            Model.SelectedMachine = Environment.MachineName;
            Model.QueueName = "TestQueue";
        };

        It should_allow_creating_the_queue = () => Model.CanAccept().ShouldBeTrue();
    }

    public class when_accepting_the_dialog : with_queue_creation_view_model
    {
        protected static TestConductorScreen Parent;
        protected static Queue AddedQueue;

        Establish context = () =>
        {
            Parent = new TestConductorScreen();
            Model.Parent = Parent;
            Model.SelectedMachine = Environment.MachineName;
            Model.QueueName = "TestQueue";
            Model.IsTransactional = true;
            
            AddedQueue = new Queue(Environment.MachineName, Model.QueueName);

            QueueManager.CreatePrivateQueue(Arg.Any<Queue>(), true).ReturnsForAnyArgs(AddedQueue);
        };

        Because of = () => Model.Accept();

        It should_create_the_queue_when_dialog_closes = () => QueueManager.Received().CreatePrivateQueue(Arg.Is<Queue>(q => q.Address.Equals("testqueue@.")), Arg.Is(true));
        It should_close_the_dialog = () => Model.IsActive.ShouldBeFalse();
    }
}