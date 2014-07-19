namespace Particular.ServiceInsight.FunctionalTests.UI.Parts
{
    using TestStack.White.UIItems;
    using TestStack.White.UIItems.Finders;
    using TestStack.White.UIItems.TreeItems;
    using TestStack.White.UIItems.WindowItems;

    public class EndpointExplorer : ProfilerElement
    {
        public EndpointExplorer(Window mainWindow) : base(mainWindow)
        {
        }

        public Tree GetTree()
        {
            var barManager = GetByAutomationId<GroupBox>("BarManager");
            var endpoint = barManager.Get<GroupBox>(SearchCriteria.ByAutomationId("EndpointExplorer"));
            var endpointTree = endpoint.Get<Tree>(SearchCriteria.ByAutomationId("EndpointTree"));

            return endpointTree;
        }
    }
}