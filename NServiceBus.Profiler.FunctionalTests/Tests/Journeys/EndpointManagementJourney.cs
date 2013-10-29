using NServiceBus.Profiler.FunctionalTests.Parts;
using NServiceBus.Profiler.FunctionalTests.ServiceControlStub;
using NUnit.Framework;
using Shouldly;

namespace NServiceBus.Profiler.FunctionalTests.Tests.Journeys
{
    public class EndpointManagementJourney : ProfilerTests
    {
        public ShellScreen Shell { get; set; }
        public ServiceControlConnectionDialog Dialog { get; set; }

        [Test]
        public void Can_connect_to_service_control_stub()
        {
            Shell.MainMenu.ToolsMenu.Click();
            //TODO: Activate Endpoint window first
            Shell.MainMenu.ConnectToManagementService.Click();

            Dialog.Activate();
            Dialog.ServiceUrl.EditableText = ServiceControl.StubServiceUrl + "/api";
            Dialog.Okay.Click();

            Wait.For(() => Dialog.IsClosed.ShouldBe(true));
        }
    }
}