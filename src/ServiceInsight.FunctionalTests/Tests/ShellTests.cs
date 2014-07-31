namespace Particular.ServiceInsight.FunctionalTests.Tests
{
    using UI.Steps;

    public class ShellTests : UITest
    {
        public DockLayoutWindowToLeft DockLayoutWindow { get; set; }

        protected override void OnApplicationInitialized()
        {
            base.OnApplicationInitialized();
            DockLayoutWindow.Execute();
        }
    }
}