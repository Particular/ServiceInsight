namespace ServiceInsight.Tests
{
    using System.Linq;
    using ConversationsData;
    using NUnit.Framework;

    [TestFixture]
    class SequenceDiagramModelCreatorTestsForJson6 : SequenceDiagramModelCreatorTestsFromJson
    {
        public SequenceDiagramModelCreatorTestsForJson6()
            : base("json6.json")
        {
        }

        [Test]
        public void DuplicateHostsShouldResultInOneLogicalEndpoint()
        {
            var sagaEndpointCount = ModelCreator.Endpoints.Count(e => e.Name == "SagaEndpoint");

            Assert.That(sagaEndpointCount, Is.EqualTo(1));
        }

        [Test]
        public void LogicalEndpointShouldHaveAllHostInformation()
        {
            var sagaEndpoint = ModelCreator.Endpoints.First(e => e.Name == "SagaEndpoint");
            var hosts = sagaEndpoint.Hosts;

            Assert.That(hosts.Count, Is.EqualTo(2));
        }
    }
}