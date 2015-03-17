namespace ServiceInsightInstaller.Managed.UI.Shell
{
    using System;
    using Caliburn.Micro;
    using MahApps.Metro.Controls;
    using ServiceInsightInstaller.Managed.Events;
    using ServiceInsightInstaller.Managed.UI.Introduction;

    class ShellViewModel : Conductor<Screen>.Collection.OneActive, IHandle<ChangePanel>
    {
        Func<IntroductionViewModel> intro;

        public ShellViewModel(Func<IntroductionViewModel> intro)
        {
            DisplayName = "ServiceInstaller Installer";

            Transition = TransitionType.Left;

            this.intro = intro;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ActivateItem(intro());
        }

        public TransitionType Transition { get; set; }

        public void Handle(ChangePanel message)
        {
            Transition = message.Transition;
            ActivateItem(message.Panel);
        }
    }
}