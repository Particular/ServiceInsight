namespace Particular.ServiceInsight.FunctionalTests.UI.Steps
{
    using Parts;

    public class DockLayoutWindowToRight : IStep
    {
        public ShellScreen Shell { get; set; }

        public void Execute()
        {
            Shell.LayoutManager.DockAutoHideGroups();
        }
    }
}