namespace ServiceInsightInstaller.Managed.UI.Introduction
{
    using System;
    using Caliburn.Micro;
    using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
    using ServiceInsightInstaller.Managed.Events;
    using ServiceInsightInstaller.Managed.UI.Progress;

    class IntroductionViewModel : Screen
    {
        private const string PackageName = "ServiceInsight";

        IEventAggregator eventAggregator;
        BootstrapperApplication ba;
        Func<ProgressViewModel> progressViewModel;

        public IntroductionViewModel(IEventAggregator eventAggregator, BootstrapperApplication ba, Func<ProgressViewModel> progressViewModel)
        {
            this.eventAggregator = eventAggregator;
            this.ba = ba;
            this.progressViewModel = progressViewModel;

            ba.DetectPackageComplete += DetectedPackage;
            ba.DetectRelatedBundle += DetectedRelatedBundle;
            ba.DetectComplete += DetectComplete;
        }

        public bool InstallVisible { get; set; }
        public bool UninstallVisible { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ba.Engine.Detect();
        }

        public void Install()
        {
            var viewModel = progressViewModel();
            viewModel.Action = LaunchAction.Install;

            eventAggregator.PublishOnCurrentThread(new ChangePanel(viewModel));
        }

        public void Uninstall()
        {
            var viewModel = progressViewModel();
            viewModel.Action = LaunchAction.Uninstall;

            eventAggregator.PublishOnCurrentThread(new ChangePanel(viewModel));
        }

        private void DetectedPackage(object sender, DetectPackageCompleteEventArgs e)
        {
            ba.Engine.Log(LogLevel.Debug, "Detected package " + e.PackageId);

            // The Package ID from the Bootstrapper chain.
            if (e.PackageId.Equals(PackageName, StringComparison.Ordinal))
            {
                if (e.State == PackageState.Present)
                {
                    UninstallVisible = true;
                }
                else
                {
                    InstallVisible = true;
                }
            }
        }

        private void DetectedRelatedBundle(object sender, DetectRelatedBundleEventArgs e)
        {
            //if (e.Operation == RelatedOperation.Downgrade)
            //{
            //    Downgrade = true;
            //}
        }

        private void DetectComplete(object sender, DetectCompleteEventArgs e)
        {
            if (ba.Command.Action == LaunchAction.Uninstall)
            {
                ba.Engine.Log(LogLevel.Verbose, "Invoking automatic plan for uninstall");
                //Execute.OnUIThread(() =>
                //{
                //    Plan(LaunchAction.Uninstall);
                //});
            }
            else if (InstallerUtils.HResultSucceeded(e.Status))
            {
                //if (Downgrade)
                //{
                //    // TODO: What behavior do we want for downgrade?
                //    State = InstallationState.DetectedNewer;
                //}

                // If we're not waiting for the user to click install, dispatch plan with the default action.
                if (ba.Command.Display != Display.Full)
                {
                    ba.Engine.Log(LogLevel.Verbose, "Invoking automatic plan for non-interactive mode.");
                    //Execute.OnUIThread(() =>
                    //{
                    //    Plan(ba.Command.Action);
                    //});
                }
            }
            else
            {
                // TODO Goto failed panel?
                //State = InstallationState.Failed;
            }
        }
    }
}