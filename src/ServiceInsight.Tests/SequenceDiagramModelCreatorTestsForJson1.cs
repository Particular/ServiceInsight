namespace Particular.ServiceInsight.Tests
{
    using NUnit.Framework;
    using Particular.ServiceInsight.Tests.ConversationsData;

    [TestFixture]
    class SequenceDiagramModelCreatorTestsForJson1 : SequenceDiagramModelCreatorTestsFromJson
    {
        public SequenceDiagramModelCreatorTestsForJson1():base("json1.json")
        {
            
        }

        [Test]
        public void FirstEndpointShouldHave1Handler()
        {
            Assert.AreEqual(1, result[0].Handlers.Count);
        }

        [Test]
        public void EndpointsShouldBeVersionedCorrectly()
        {
            Assert.AreEqual(1, result[0].Handlers.Count);
            Assert.AreEqual("5.2.3", result[0].Version);
            Assert.AreEqual("5.2.3", result[1].Version);
            Assert.AreEqual(null, result[2].Version);
        }

        [Test]
        public void EndpointsAreDeDup()
        {
            Assert.AreEqual(3, result.Count);
        }
    }
}