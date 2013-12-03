using System.Collections.Generic;
using Caliburn.PresentationFramework.Filters;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public interface IConnectToMachineViewModel : IScreen, IWorkTracker
    {
        string ComputerName { get; }
        bool IsAddressValid { get; }
        string ProgressMessage { get; }
        IList<string> Machines { get; }

        bool CanAccept();
        void Accept();
        void Close();
    }

    public class ConnectToMachineViewModel : Screen, IConnectToMachineViewModel
    {
        public const string DiscoveringComputersOnNetwork = "Discovering network computers...";

        private readonly INetworkOperations _networkOperations;

        public ConnectToMachineViewModel(INetworkOperations networkOperations)
        {
            _networkOperations = networkOperations;
            Machines = new List<string>();
            DisplayName = "Connect To MSMQ";
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            StartWorkInProgress(DiscoveringComputersOnNetwork);
            IsAddressValid = true;

            var machines = await _networkOperations.GetMachines();

            Machines = new List<string>(machines);
            StopWorkInProgress();
        }

        public string ComputerName { get; set; }
        public IList<string> Machines { get; private set; }
        public bool IsAddressValid { get; set; }
        public bool WorkInProgress { get; private set; }
        public string ProgressMessage { get; set; }

        public void Close()
        {
            TryClose(false);
        }

        public bool CanAccept()
        {
            return !string.IsNullOrEmpty(ComputerName);
        }

        [AutoCheckAvailability]
        public void Accept()
        {
            IsAddressValid = Address.IsValidAddress(ComputerName);
            if (IsAddressValid)
            {
                TryClose(true);
            }
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