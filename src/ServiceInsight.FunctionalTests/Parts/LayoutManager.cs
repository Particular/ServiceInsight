namespace Particular.ServiceInsight.FunctionalTests.Parts
{
    using System.Threading;
    using System.Windows;
    using Extensions;
    using TestStack.White.UIItems;
    using TestStack.White.UIItems.Finders;
    using TestStack.White.UIItems.TreeItems;
    using TestStack.White.UIItems.WindowItems;

    public class LayoutManager : ProfilerElement
    {
        private readonly GroupBox _barManager;
        private readonly IUIItem[] _autoHideGroups;

        public LayoutManager(Window mainWindow) : base(mainWindow)
        {
            _barManager = mainWindow.Get<GroupBox>("BarManager");
            _autoHideGroups = _barManager.GetMultiple(SearchCriteria.ByClassName("AutoHideGroup"));
        }

        public void DockAutoHideGroups()
        {
            foreach (var item in _autoHideGroups)
            {
                Dock(item);
                Thread.Sleep(1000);
            }
        }

        private void Dock(IUIItem item)
        {
            //NOTE: Workaround. Can not find dockmanager's context menu
            item.RightClick();
            Mouse.Location = new Point(Mouse.Location.X + 10, Mouse.Location.Y + 10); //First item in the menu
            Mouse.Click();
        }

        public void ActivateQueueExplorer()
        {
            var queueExplorer = _barManager.Get<GroupBox>(SearchCriteria.ByClassName("LayoutPanel").AndAutomationId("QueueExplorer"));
            var computerNode = (TreeNode)queueExplorer.Get(SearchCriteria.ByText("hadi-pc"));
            
            computerNode.Focus();
            computerNode.CollapseNode();
            computerNode.ExpandNode();
        }

        public void ActivateEndpointExplorer()
        {
            var endpoint = ServiceControlStub.ServiceControl.StubServiceUrl;
            var queueExplorer = _barManager.Get<GroupBox>(SearchCriteria.ByClassName("LayoutPanel").AndAutomationId("QueueExplorer"));
            var computerNode = (TreeNode)queueExplorer.Get(SearchCriteria.ByText(endpoint));

            computerNode.Focus();
            computerNode.CollapseNode();
            computerNode.ExpandNode();
        }
    }
}