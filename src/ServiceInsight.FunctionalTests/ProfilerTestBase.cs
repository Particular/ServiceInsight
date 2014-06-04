namespace Particular.ServiceInsight.FunctionalTests
{
    using Parts;

    public class ProfilerTestBase : TestBase
    {
        public ShellScreen Shell { get; set; }

        protected override void OnSetupUI()
        {
            Shell.LayoutManager.DockAutoHideGroups();
        }
    }
}