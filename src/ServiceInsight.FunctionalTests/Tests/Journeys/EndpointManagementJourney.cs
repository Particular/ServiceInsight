namespace Particular.ServiceInsight.FunctionalTests.Tests.Journeys
{
    using Desktop.Shell;
    using NUnit.Framework;
    using Parts;
    using ServiceControlStub;
    using Shouldly;

    public class EndpointManagementJourney : ProfilerTestBase
    {
        public ServiceControlConnectionDialog Dialog { get; set; }

        [Test]
        public void Can_connect_to_service_control_stub()
        {
            Shell.MainMenu.ToolsMenu.Click();
            Shell.MainMenu.ConnectToServiceControl.Click();

            Dialog.Activate();
            Dialog.ServiceUrl.EditableText = ServiceControl.GetUrl();
            Dialog.Okay.Click();

            Wait.For(() => Dialog.IsClosed.ShouldBe(true));

            Shell.StatusBar.GetStatusMessage().ShouldBe(StatusBarManager.DoneStatusMessage);
        }
    }
}