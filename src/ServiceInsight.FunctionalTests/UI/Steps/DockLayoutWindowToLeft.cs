namespace ServiceInsight.FunctionalTests.UI.Steps
{
    using Parts;

    public class DockLayoutWindowToLeft : IStep
    {
        public ShellScreen Shell { get; set; }

        public void Execute()
        {
            Shell.LayoutManager.DockAutoHideGroups();
        }
    }
}