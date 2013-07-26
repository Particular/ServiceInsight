using System.Collections.Generic;
using Caliburn.PresentationFramework.Filters;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.Explorer.QueueExplorer;
using NServiceBus.Profiler.Desktop.ExtensionMethods;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public interface IQueueCreationViewModel : IScreen, IWorkTracker
    {
        string QueueName { get; set; }
        string SelectedMachine { get; set; }
        bool IsTransactional { get; set; }
        List<string> Machines { get; }
        bool CanAccept();
        bool CreateQueue();
    }

    public class QueueCreationViewModel : Screen, IQueueCreationViewModel
    {
        private readonly IQueueManager _queueManager;
        private readonly IQueueExplorerViewModel _explorer;
        private readonly INetworkOperations _networkOperations;

        public QueueCreationViewModel(
            IQueueManager queueManager, 
            IQueueExplorerViewModel explorer,
            INetworkOperations networkOperations)
        {
            _queueManager = queueManager;
            _explorer = explorer;
            _networkOperations = networkOperations;
            Machines = new List<string>();
            DisplayName = "Queue";
            IsTransactional = true;
        }

        public virtual string QueueName { get; set; }

        public virtual string SelectedMachine { get; set; }

        public virtual bool IsTransactional { get; set; }

        public virtual List<string> Machines { get; private set; }

        protected override async void OnActivate()
        {
            base.OnActivate();

            WorkInProgress = true;
            Machines.Clear();
            SelectedMachine = _explorer.ConnectedToAddress;
            var machines = await _networkOperations.GetMachines();
            Machines.AddRange(machines);
            WorkInProgress = false;
        }

        public virtual void Close()
        {
            TryClose(false);
        }

        public virtual bool CanAccept()
        {
            return !QueueName.IsEmpty() &&
                   !SelectedMachine.IsEmpty();
        }

        [AutoCheckAvailability]
        public virtual void Accept()
        {
            if (CreateQueue())
            {
                TryClose(true);
            }
        }

        public bool CreateQueue()
        {
            var queue = _queueManager.CreatePrivateQueue(new Queue(SelectedMachine, QueueName), IsTransactional);
            return queue != null;
        }

        public bool WorkInProgress { get; private set; }
    }
}