namespace Particular.ServiceInsight.FunctionalTests.Parts
{
    using System.Threading;
    using System.Windows;
    using Shouldly;
    using TestStack.White.UIItems;
    using TestStack.White.UIItems.Finders;
    using TestStack.White.UIItems.WindowItems;

    public class LayoutManager : ProfilerElement
    {
        private readonly GroupBox barManager;
        private readonly IUIItem[] autoHideGroups;

        public LayoutManager(Window mainWindow) : base(mainWindow)
        {
            barManager = mainWindow.Get<GroupBox>("BarManager");
            autoHideGroups = barManager.GetMultiple(SearchCriteria.ByClassName("AutoHideGroup"));
        }

        public void DockAutoHideGroups()
        {
            foreach (var item in autoHideGroups)
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

        public void ActivateEndpointExplorer()
        {
            var endpointExplorer = barManager.Get<GroupBox>(SearchCriteria.ByClassName("LayoutPanel").AndAutomationId("EndpointExplorer"));

            endpointExplorer.ShouldNotBe(null);

            Dock(endpointExplorer);
        }
    }
}