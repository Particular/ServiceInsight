namespace Particular.ServiceInsight.Tests
{
    using NUnit.Framework;
    using Particular.ServiceInsight.Tests.ConversationsData;

    [TestFixture]
    class SequenceDiagramModelCreatorTestsForJson2 : SequenceDiagramModelCreatorTestsFromJson
    {
        public SequenceDiagramModelCreatorTestsForJson2():base("json2.json")
        {
            
        }

        [Test]
        public void FirstEndpointShouldHave3Handler()
        {
            Assert.AreEqual(4, result[0].Handlers.Count);
            Assert.AreEqual("StartOrder", result[0].Handlers[0].Name);
            Assert.AreEqual("CoolingOff", result[0].Handlers[1].Name);
            Assert.AreEqual("CompleteOrder", result[0].Handlers[2].Name);
        }
    }
}