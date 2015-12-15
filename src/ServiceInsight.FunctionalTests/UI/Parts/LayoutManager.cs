namespace ServiceInsight.FunctionalTests.UI.Parts
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Windows;
    using System.Windows.Automation;
    using Shouldly;
    using TestStack.White.UIItems;
    using TestStack.White.UIItems.Finders;
    using TestStack.White.UIItems.WPFUIItems;

    public class LayoutManager : UIElement
    {
        public void DockAutoHideGroups()
        {
            foreach (var item in AutoHideGroups())
            {
                Dock(item);
                Thread.Sleep(1000);
            }
        }

        GroupBox BarManager()
        {
            var barManager = MainWindow.Get<GroupBox>("BarManager");
            barManager.ShouldNotBe(null);
            return barManager;
        }

        IEnumerable<IUIItem> AutoHideGroups()
        {
            return BarManager().GetMultiple(SearchCriteria.ByClassName("AutoHideGroup"));
        }

        void Dock(IUIItem item)
        {
            //TODO: Workaround. Can not find dockmanager's context menu. Try to remove as it should have been fixed now.
            item.RightClick();
            Mouse.Location = new Point(Mouse.Location.X + 10, Mouse.Location.Y + 10); //First item in the menu
            Mouse.Click();
        }

        public void ActivateEndpointExplorer()
        {
            var endpointExplorer = BarManager().Get<GroupBox>(SearchCriteria.ByClassName("LayoutPanel").AndAutomationId("EndpointExplorer"));
            endpointExplorer.ShouldNotBe(null);

            Dock(endpointExplorer);
        }

        public void SelectFlowDiagramTab()
        {
            SelectTab("MessageFlow");
        }

        public void SelectSagaTab()
        {
            SelectTab("SagaWindow");
        }

        public void SelectHeadersTab()
        {
            SelectTab("MessageHeaders");
        }

        public void SelectBodyTab()
        {
            SelectTab("MessageBody");
        }

        public void SelectLogsTab()
        {
            SelectTab("LogWindow");
        }

        private void SelectTab(string automationId)
        {
            var tabbedGroup = BarManager().Get(SearchCriteria.ByControlType(ControlType.Tab).AndAutomationId("MainTabbedView"));
            tabbedGroup.ShouldNotBe(null);

            var tabToSelect = tabbedGroup.Get<Button>(automationId + "TabButtonId");
            tabToSelect.ShouldNotBe(null);

            tabToSelect.Click();
        }
    }
}