namespace Particular.ServiceInsight.FunctionalTests.Tests.Journeys
{
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
            Shell.LayoutManager.ActivateQueueExplorer();

            Shell.MainMenu.ToolsMenu.Click();
            Shell.MainMenu.ConnectToServiceControl.Click();

            Dialog.Activate();
            Dialog.ServiceUrl.EditableText = ServiceControl.StubServiceUrl + "/api";
            Dialog.Okay.Click();

            Wait.For(() => Dialog.IsClosed.ShouldBe(true));
        }
    }
}