using System.Windows.Automation;
using TestStack.White.UIItems.TreeItems;

namespace NServiceBus.Profiler.FunctionalTests.Extensions
{
    public static class TreeNodeExtensions
    {
        public static void ExpandNode(this TreeNode node)
        {
            var patterns = (ExpandCollapsePattern)node.AutomationElement.GetCurrentPattern(AutomationPattern.LookupById(ExpandCollapsePatternIdentifiers.Pattern.Id));
            patterns.Expand();
        }

        public static void CollapseNode(this TreeNode node)
        {
            var patterns = (ExpandCollapsePattern)node.AutomationElement.GetCurrentPattern(AutomationPattern.LookupById(ExpandCollapsePatternIdentifiers.Pattern.Id));
            patterns.Expand();
        }
    }
}