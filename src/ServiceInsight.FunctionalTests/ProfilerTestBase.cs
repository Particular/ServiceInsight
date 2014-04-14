using NServiceBus.Profiler.FunctionalTests.Parts;

namespace NServiceBus.Profiler.FunctionalTests
{
    public class ProfilerTestBase : TestBase
    {
        public ShellScreen Shell { get; set; }

        protected override void OnSetupUI()
        {
            Shell.LayoutManager.DockAutoHideGroups();
        }
    }
}