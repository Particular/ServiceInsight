namespace Particular.ServiceInsight.FunctionalTests.Tests
{
    using Services;
    using UI.Parts;
    using UI.Steps;
    using NUnit.Framework;
    using Shouldly;

    namespace Particular.ServiceInsight.FunctionalTests.Tests
    {
        using TestStack.BDDfy;
        using TestStack.BDDfy.Scanners.StepScanners.Fluent;

        public class EndpointExplorerTests : UITest
        {
            public ShellScreen Shell { get; set; }
            public ConnectToServiceControl ConnectToServiceControl { get; set; }
            public EndpointExplorer EndpointExplorer { get; set; }

            [Test]
            public void ConnectingToServiceControlAndDisplayingTheEndpoints()
            {
                this.Given(s => s.ThereAreTwoEndpointsAvailable())
                    .When(s => s.ConnectedToServiceControl())
                    .Then(s => s.ServiceControlVersionIsDisplayedOnStatusBar())
                    .And(s => s.EndpointsAreDisplayedInEndpointExplorer())
                    .BDDfy();
            }

            void ServiceControlVersionIsDisplayedOnStatusBar()
            {
                Shell.StatusBar.GetStatusMessage().ShouldContain(FakeServiceControl.Version);                
            }

            void ConnectedToServiceControl()
            {
                ConnectToServiceControl.Execute();
            }

            void ThereAreTwoEndpointsAvailable()
            {
                TestDataBuilder.EndpointBuilder().WithEndpoints("Sales", "CustomerRelations").Build();                
            }

            void EndpointsAreDisplayedInEndpointExplorer()
            {
                var connectionNode = EndpointExplorer.GetConnectionNode();
                connectionNode.Text.ShouldContain(FakeServiceControl.Address);

                var endpointNames = EndpointExplorer.GetEndpointNames();

                endpointNames.Count.ShouldBe(2);
                endpointNames.ShouldContain("Sales");
                endpointNames.ShouldContain("CustomerRelations");                
            }
        }
    }
}