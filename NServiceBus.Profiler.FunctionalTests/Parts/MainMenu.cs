using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.WindowStripControls;
using TestStack.White.UIItems.WPFUIItems;

namespace NServiceBus.Profiler.FunctionalTests.Parts
{
    public class MainMenu : ProfilerElement
    {
        public MainMenu(IMainWindow mainWindow) : base(mainWindow)
        {
        }

        public MenuBar ToolsMenu
        {
            get { return GetMenu(Desktop.Screens.Shell.Tools); }
        }

        public MenuBar FileMenu
        {
            get { return GetMenu(Desktop.Screens.Shell.File); }
        }

        public MenuBar ViewMenu
        {
            get { return GetMenu(Desktop.Screens.Shell.View); }
        }

        public MenuBar HelpMenu
        {
            get { return GetMenu(Desktop.Screens.Shell.Help); }
        }

        public Button CreateQueue
        {
            get { return GetMenuItem(ToolsMenu, Desktop.Screens.Shell.CreateQueueMenu); }
        }

        public Button ConnectToManagementService
        {
            get { return GetMenuItem(ToolsMenu, Desktop.Screens.Shell.ConnectToManagementServiceMenu); }
        }

        public GroupBox BarManager
        {
            get { return GetByAutomationId<GroupBox>(Desktop.Screens.Shell.BarManager); }
        }

        private MenuBar GetMenu(string name)
        {
            return BarManager.Get<MenuBar>(name);
        }

        private Button GetMenuItem(MenuBar menu, string name)
        {
            return menu.Get<Button>(SearchCriteria.ByAutomationId(name));
        }
    }
}