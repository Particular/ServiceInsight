namespace Particular.ServiceInsight.FunctionalTests.Extensions
{
    using System.Windows.Automation;
    using TestStack.White.UIItems.TreeItems;

    public static class TreeNodeExtensions
    {
        public static void ExpandNode(this TreeNode node)
        {
            var patterns = (ExpandCollapsePattern)node.AutomationElement.GetCurrentPattern(ExpandCollapsePattern.Pattern);
            patterns.Expand();
        }

        public static void CollapseNode(this TreeNode node)
        {
            var patterns = (ExpandCollapsePattern)node.AutomationElement.GetCurrentPattern(ExpandCollapsePattern.Pattern);
            patterns.Collapse();
        }
    }
}