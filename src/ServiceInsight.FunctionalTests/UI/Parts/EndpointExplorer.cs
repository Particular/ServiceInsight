namespace Particular.ServiceInsight.FunctionalTests.UI.Parts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Shouldly;
    using TestStack.White.UIItems;
    using TestStack.White.UIItems.Finders;
    using TestStack.White.UIItems.TreeItems;
    using TestStack.White.UIItems.WindowItems;

    public class EndpointExplorer : UIElement
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

        public TreeNode GetConnectionNode()
        {
            var tree = GetTree();
            tree.Nodes.Count.ShouldBe(1);
            return tree.Nodes[0];
        }

        public IList<TreeNode> GetEndpointNodes()
        {
            var connectionNode = GetConnectionNode();
            return connectionNode.Nodes.ToList();
        }

        public IList<string> GetEndpointNames()
        {
            var nodes = GetEndpointNodes();
            return nodes.Select(n => n.Text).ToList();
        }

        public void SelectEndpoint(string endpoint)
        {
            var tree = GetTree();
            var root = tree.Nodes[0];
            var endpointNode = root.Nodes.First(n => n.Text.Equals(endpoint, StringComparison.InvariantCultureIgnoreCase));

            endpointNode.Click();
        }
    }

    public class MessagesWindow : UIElement
    {
        public MessagesWindow(Window mainWindow) : base(mainWindow)
        {
        }

        public int GetMessageCount()
        {
            var barManager = GetByAutomationId<GroupBox>("BarManager");
            var grid = barManager.Get<ListView>(SearchCriteria.ByAutomationId("grid"));

            return grid.Rows.Count;

        }
    }
}