﻿using System.Collections.Generic;
using Caliburn.PresentationFramework.Filters;
using Caliburn.PresentationFramework.Screens;
using Particular.ServiceInsight.Desktop.Core;
using Particular.ServiceInsight.Desktop.Models;

namespace Particular.ServiceInsight.Desktop.Shell
{
    public class ConnectToMachineViewModel : Screen, IWorkTracker
    {
        private readonly INetworkOperations _networkOperations;

        public ConnectToMachineViewModel(INetworkOperations networkOperations)
        {
            _networkOperations = networkOperations;
            Machines = new List<string>();
            DisplayName = "Connect To Queue";
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            WorkInProgress = true;

            var machines = await _networkOperations.GetMachines();

            Machines = new List<string>(machines);
            IsAddressValid = true;
            WorkInProgress = false;
        }

        public virtual string ComputerName { get; set; }

        public virtual void Close()
        {
            TryClose(false);
        }

        public virtual bool CanAccept()
        {
            return !string.IsNullOrEmpty(ComputerName);
        }

        public List<string> Machines { get; private set; }

        public bool IsAddressValid { get; set; }

        [AutoCheckAvailability]
        public virtual void Accept()
        {
            IsAddressValid = Address.IsValidAddress(ComputerName);
            if (IsAddressValid)
            {
                TryClose(true);
            }
        }

        public bool WorkInProgress { get; private set; }
    }
}