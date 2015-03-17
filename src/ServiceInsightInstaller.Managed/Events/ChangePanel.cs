namespace ServiceInsightInstaller.Managed.Events
{
    using Caliburn.Micro;
    using MahApps.Metro.Controls;

    class ChangePanel
    {
        public ChangePanel(Screen panel, TransitionType transition = TransitionType.Left)
        {
            Panel = panel;
            Transition = transition;
        }

        public Screen Panel { get; private set; }

        public TransitionType Transition { get; private set; }
    }
}