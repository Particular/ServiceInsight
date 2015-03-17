namespace ServiceInsightInstaller.Managed.UI.Progress
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Caliburn.Micro;
    using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
    using ServiceInsightInstaller.Managed.Events;
    using ServiceInsightInstaller.Managed.UI.Finished;

    class ProgressViewModel : Screen
    {
        private const string NetFxPackageName = "NetFx45Redist";

        IEventAggregator eventAggregator;
        BootstrapperApplication ba;
        Func<FinishedViewModel> finishedViewModel;
        Dictionary<string, int> downloadRetries;

        public ProgressViewModel(IEventAggregator eventAggregator, BootstrapperApplication ba, Func<FinishedViewModel> finishedViewModel)
        {
            this.eventAggregator = eventAggregator;
            this.ba = ba;
            this.finishedViewModel = finishedViewModel;

            downloadRetries = new Dictionary<string, int>();

            Action = LaunchAction.Install;

            ba.PlanPackageBegin += PlanPackageBegin;
            ba.PlanComplete += PlanComplete;

            ba.ApplyBegin += ApplyBegin;
            ba.ApplyComplete += ApplyComplete;

            ba.ResolveSource += ResolveSource;

            ba.ExecuteMsiMessage += ExecuteMsiMessage;
            ba.CacheAcquireProgress += CacheAcquireProgress;
            ba.CacheComplete += CacheComplete;
            ba.ExecuteProgress += ApplyExecuteProgress;
        }

        public LaunchAction Action { get; set; }

        public string Message { get; set; }

        public int CacheProgress { get; set; }
        public int ApplyProgress { get; set; }

        public int OverallProgress
        {
            get { return (CacheProgress + ApplyProgress) / 2; }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ba.Engine.Plan(Action);
        }

        public override void CanClose(Action<bool> callback)
        {
            // Cannot close while this view is active.
            callback(false);
        }

        private void PlanPackageBegin(object sender, PlanPackageBeginEventArgs e)
        {
            // Turns off .NET install when setting up the install plan as we already have it.
            //if (e.PackageId.Equals(ba.Engine.StringVariables["WixMbaPrereqPackageId"], StringComparison.Ordinal))
            if (e.PackageId.Equals(NetFxPackageName, StringComparison.Ordinal))
            {
                e.State = RequestState.None;
            }
        }

        private void PlanComplete(object sender, PlanCompleteEventArgs e)
        {
            if (InstallerUtils.HResultSucceeded(e.Status))
            {
                //PreApplyState = State;
                //State = InstallationState.Applying;
                //ba.Engine.Apply(hwnd);

                ba.Engine.Apply(InstallerUtils.HWnd);
            }
            else
            {
                //State = InstallationState.Failed;
            }
        }

        private void ApplyBegin(object sender, ApplyBeginEventArgs e)
        {
            downloadRetries.Clear();
        }

        private void ApplyComplete(object sender, ApplyCompleteEventArgs e)
        {
            // If we're not in Full UI mode, we need to alert the dispatcher to stop and close the window for passive.
            if (ba.Command.Display != Display.Full)
            {
                // If its passive, send a message to the window to close.
                if (ba.Command.Display == Display.Passive)
                {
                    ba.Engine.Log(LogLevel.Verbose, "Automatically closing the window for non-interactive install");
                    TryClose();
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }

            // Set the state to applied or failed unless the state has already been set back to the preapply state
            // which means we need to show the UI as it was before the apply started.
            //if (State != PreApplyState)
            //{
            //    State = HResultSucceeded(e.Status) ? InstallationState.Applied : InstallationState.Failed;
            //}

            if (InstallerUtils.HResultSucceeded(e.Status))
            {
                eventAggregator.PublishOnUIThread(new ChangePanel(finishedViewModel()));
            }
        }

        private void ResolveSource(object sender, ResolveSourceEventArgs e)
        {
            int retries;

            downloadRetries.TryGetValue(e.PackageOrContainerId, out retries);
            downloadRetries[e.PackageOrContainerId] = retries + 1;

            e.Result = retries < 3 && !String.IsNullOrEmpty(e.DownloadSource) ? Result.Download : Result.Ok;
        }

        private void ExecuteMsiMessage(object sender, ExecuteMsiMessageEventArgs e)
        {
            lock (this)
            {
                Message = e.Message;
            }
        }

        void CacheAcquireProgress(object sender, CacheAcquireProgressEventArgs e)
        {
            lock (this)
            {
                CacheProgress = e.OverallPercentage;
            }
        }

        void CacheComplete(object sender, CacheCompleteEventArgs e)
        {
            lock (this)
            {
                CacheProgress = 100;
            }
        }

        void ApplyExecuteProgress(object sender, ExecuteProgressEventArgs e)
        {
            lock (this)
            {
                ApplyProgress = e.OverallPercentage;
            }
        }
    }
}