namespace Particular.ServiceInsight.FunctionalTests.Tests
{
    using NUnit.Framework;
    using Shouldly;
    using UI.Parts;

    public class EndpointManagementJourney : UITest
    {
        public ServiceControlConnectionDialog Dialog { get; set; }
        public ShellScreen Shell { get; set; }

        [Test]
        public void Can_connect_to_service_control_fake()
        {
            Shell.MainMenu.ToolsMenu.Click();
            Shell.MainMenu.ConnectToServiceControl.Click();

            Dialog.Activate();
            Dialog.ServiceUrl.EditableText = "http://localhost";
            Dialog.Okay.Click();

            Wait.For(() => Dialog.IsClosed.ShouldBe(true));

            Shell.StatusBar.GetStatusMessage().ShouldContain(FakeServiceControl.Version);
        }


    }
}