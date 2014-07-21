namespace Particular.ServiceInsight.FunctionalTests.UI.Steps
{
    using Parts;
    using Services;
    using Shouldly;
    using TestStack.White.InputDevices;
    using TestStack.White.WindowsAPI;

    public class ConnectToServiceControl : IStep
    {
        public ShellScreen Shell { get; set; }
        public ServiceControlConnectionDialog Dialog { get; set; }
        public Waiter Wait { get; set; }
        public IKeyboard Keyboard { get; set; }

        public void Execute()
        {
            Shell.MainMenu.ToolsMenu.Click();
            Shell.MainMenu.ConnectToServiceControl.Click();

            Dialog.Activate();
            Dialog.ServiceUrl.EditableText = "http://" + FakeServiceControl.Address;
            Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.DELETE); //NOTE: To delete any auto-completed text on the dropdown
            Dialog.Okay.Click();

            Wait.For(() => Dialog.IsClosed.ShouldBe(true));
        }
    }
}