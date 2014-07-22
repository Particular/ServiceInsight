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

         [SetUp]
         public void Initialize()
         {
             var sales = new Endpoint { Name = "Sales" };
             var customerRelations = new Endpoint { Name = "CustomerRelations" };

             TestDataBuilder.EndpointBuilder().WithEndpoints(sales, customerRelations).Build();
         }

         [Test]
         public void Can_connect_to_fake_service_control_service()
         {
             ConnectToServiceControl.Execute();

             Shell.StatusBar.GetStatusMessage().ShouldContain(FakeServiceControl.Version);
         }

         [Test]
         public void Can_see_service_control_address_and_available_endpoint_names_in_explorer()
         {
             ConnectToServiceControl.Execute();

             var tree = EndpointExplorer.GetTree();
             var rootNode = tree.Nodes[0];
             rootNode.Text.ShouldContain(FakeServiceControl.Address);

             var children = rootNode.Nodes.Select(n => n.Text).ToList();

             children.Count.ShouldBe(2);
             children.ShouldContain("Sales");
             children.ShouldContain("CustomerRelations");
         }
     }
 }
}