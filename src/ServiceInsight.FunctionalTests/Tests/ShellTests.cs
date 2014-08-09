namespace Particular.ServiceInsight.FunctionalTests.Tests
{
    using UI.Steps;

    public abstract class ShellTests : UITest
    {
        public DockLayoutWindowToLeft DockLayoutWindow { get; set; }

        protected override void OnApplicationInitialized()
        {
            DockLayoutWindow.Execute();
        }
    }
}