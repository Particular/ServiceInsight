namespace Particular.ServiceInsight.FunctionalTests.UI.Steps
{
    using Parts;

    public class DockLayoutManager : IStep
    {
        public ShellScreen Shell { get; set; }

        public void Execute()
        {
            Shell.LayoutManager.DockAutoHideGroups();
        }
    }
}