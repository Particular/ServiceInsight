namespace Particular.ServiceInsight.FunctionalTests.UI.Steps
{
    using Parts;
    using Services;
    using Shouldly;

    public class ConnectToServiceControl : IStep
    {
        public ShellScreen Shell { get; set; }
        public ServiceControlConnectionDialog Dialog { get; set; }
        public Waiter Wait { get; set; }

        public void Execute()
        {
            Shell.MainMenu.ToolsMenu.Click();
            Shell.MainMenu.ConnectToServiceControl.Click();

            Dialog.Activate();
            Dialog.ServiceUrl.EditableText = "http://localhost";
            Dialog.Okay.Click();

            Wait.For(() => Dialog.IsClosed.ShouldBe(true));
        }
    }
}