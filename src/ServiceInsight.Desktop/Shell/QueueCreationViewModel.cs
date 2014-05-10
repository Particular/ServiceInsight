namespace Particular.ServiceInsight.Desktop.Shell
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Caliburn.PresentationFramework.Filters;
    using Caliburn.PresentationFramework.Screens;
    using Core;
    using Explorer.QueueExplorer;
    using ExtensionMethods;
    using Models;

    public interface IQueueCreationViewModel : IScreen, IWorkTracker
    {
        string QueueName { get; set; }
        string SelectedMachine { get; set; }
        bool IsTransactional { get; set; }
        List<string> Machines { get; }
        bool CanAccept();
        Task<bool> CreateQueue();
    }

    public class QueueCreationViewModel : Screen, IQueueCreationViewModel
    {
        public const string DiscoveringComputersOnNetwork = "Discovering network computers...";

        private readonly IQueueManagerAsync _queueManager;
        private readonly IQueueExplorerViewModel _explorer;
        private readonly INetworkOperations _networkOperations;

        public QueueCreationViewModel(
            IQueueManagerAsync queueManager, 
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

        public string ProgressMessage { get; private set; }
        public bool WorkInProgress { get; private set; }
        public string QueueName { get; set; }
        public string SelectedMachine { get; set; }
        public bool IsTransactional { get; set; }
        public List<string> Machines { get; private set; }

        protected override async void OnActivate()
        {
            base.OnActivate();

            StartWorkInProgress(DiscoveringComputersOnNetwork);
            Machines.Clear();
            SelectedMachine = _explorer.ConnectedToAddress;
            var machines = await _networkOperations.GetMachines();
            Machines.AddRange(machines);
            StopWorkInProgress();
        }

        public void Close()
        {
            TryClose(false);
        }

        public bool CanAccept()
        {
            return !QueueName.IsEmpty() &&
                   !SelectedMachine.IsEmpty();
        }

        [AutoCheckAvailability]
        public async void Accept()
        {
            var created = await CreateQueue();
            if (created)
            {
                TryClose(true);
            }
        }

        public async Task<bool> CreateQueue()
        {
            var queue = await _queueManager.CreatePrivateQueueAsync(new Queue(SelectedMachine, QueueName), IsTransactional);
            return queue != null;
        }

        private void StartWorkInProgress(string message)
        {
            ProgressMessage = message;
            WorkInProgress = true;
        }

        private void StopWorkInProgress()
        {
            ProgressMessage = string.Empty;
            WorkInProgress = false;
        }
    }
}