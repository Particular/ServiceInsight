namespace Particular.ServiceInsight.FunctionalTests.Tests
{
    using System.Linq;
    using Desktop.Models;
    using Services;
    using UI.Parts;
    using UI.Steps;
    using NUnit.Framework;
    using Shouldly;

    namespace Particular.ServiceInsight.FunctionalTests.Tests
 {
     public class EndpointExplorerTests : UITest
     {
         public ShellScreen Shell { get; set; }
         public ConnectToServiceControl ConnectToServiceControl { get; set; }
         public EndpointExplorer EndpointExplorer { get; set; }

         [Test]
         public void Can_connect_to_fake_service_control_service()
         {
             ConnectToServiceControl.Execute();

             Shell.StatusBar.GetStatusMessage().ShouldContain(FakeServiceControl.Version);
         }

         [Test]
         public void Can_see_service_control_address_and_available_endpoint_names_in_explorer()
         {
             var sales = new Endpoint { Name = "Sales" };
             var customerRelations = new Endpoint { Name = "CustomerRelations" };

             TestDataBuilder.EndpointBuilder().WithEndpoints(sales, customerRelations).Build();

             ConnectToServiceControl.Execute();

             var rootNode = EndpointExplorer.GetTree().Nodes[0];
             rootNode.Text.ShouldContain(FakeServiceControl.Address);

             var endpoints = rootNode.Nodes.Select(n => n.Text).ToList();
             endpoints.Count.ShouldBe(2);
             endpoints.ShouldContain(sales.Name);
             endpoints.ShouldContain(customerRelations.Name);
         }
     }
 }
}