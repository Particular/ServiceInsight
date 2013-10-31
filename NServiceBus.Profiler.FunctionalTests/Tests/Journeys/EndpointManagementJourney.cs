using NServiceBus.Profiler.FunctionalTests.Parts;
using NServiceBus.Profiler.FunctionalTests.ServiceControlStub;
using NUnit.Framework;
using Shouldly;

namespace NServiceBus.Profiler.FunctionalTests.Tests.Journeys
{
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