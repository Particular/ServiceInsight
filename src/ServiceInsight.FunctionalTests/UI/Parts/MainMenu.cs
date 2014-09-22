namespace Particular.ServiceInsight.FunctionalTests.UI.Parts
{
    using Shouldly;
    using TestStack.White.UIItems;
    using TestStack.White.UIItems.Finders;
    using TestStack.White.UIItems.WPFUIItems;

    public class MainMenu : UIElement
    {
        public void ClickToolsMenu()
        {
            GetMenu("ToolsMenu");
        }

        public void ClickFileMenu()
        {
            GetMenu("FileMenu");
        }

        public void ClickViewMenu()
        {
            GetMenu("ViewMenu");
        }

        public void ClickHelpMenu()
        {
            GetMenu("HelpMenu");
        }

        public Button ConnectToServiceControl
        {
            get { return GetMenuItem("ConnectToServiceControlMenuItem"); }
        }

        public GroupBox BarManager
        {
            get { return GetByAutomationId<GroupBox>("BarManager"); }
        }

        void GetMenu(string name)
        {
            var mainMenu = BarManager.Get(SearchCriteria.ByAutomationId("MainMenuBar"));
            mainMenu.ShouldNotBe(null);

            var topLevelMenu = mainMenu.Get(SearchCriteria.ByAutomationId(name));
            topLevelMenu.ShouldNotBe(null);

            topLevelMenu.Click();
        }

        Button GetMenuItem(string name)
        {
            return BarManager.Get<Button>(SearchCriteria.ByAutomationId(name));
        }
    }
}