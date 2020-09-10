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
        public void DuplicateHostsShuldResultInOneLogicalEndpoint()
        {
            var sagaEndpoint = ModelCreator.Endpoints.Count(e => e.Name == "SagaEndpoint");

            Assert.IsTrue(sagaEndpoint == 1);
        }
    }
}