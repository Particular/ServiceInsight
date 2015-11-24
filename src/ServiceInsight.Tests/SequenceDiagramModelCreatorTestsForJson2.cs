namespace Particular.ServiceInsight.Tests
{
    using System.Linq;
    using NUnit.Framework;
    using Particular.ServiceInsight.Tests.ConversationsData;

    [TestFixture]
    class SequenceDiagramModelCreatorTestsForJson2 : SequenceDiagramModelCreatorTestsFromJson
    {
        public SequenceDiagramModelCreatorTestsForJson2() : base("json2.json")
        {
        }

        [Test]
        public void EndpointShouldHave4Handler()
        {
            Assert.AreEqual(4, result[0].Handlers.Count);
            Assert.AreEqual(null, result[0].Handlers[0].Name);
            Assert.AreEqual("StartOrder", result[0].Handlers[1].Name);
            Assert.AreEqual("CoolingOff", result[0].Handlers[2].Name);
            Assert.AreEqual("CompleteOrder", result[0].Handlers[3].Name);
        }

        [Test]
        public void ArrowsHaveFromAndToPopulated()
        {
            Assert.IsNull(result[0].Handlers[0].In);
            Assert.AreEqual(result[0].Handlers[0], result[0].Handlers[0].Out.First().FromHandler);
            Assert.AreEqual(result[0].Handlers[1], result[0].Handlers[0].Out.First().ToHandler);

            Assert.AreEqual(result[0].Handlers[0], result[0].Handlers[1].In.FromHandler);
            Assert.AreEqual(result[0].Handlers[1], result[0].Handlers[1].In.ToHandler);
            Assert.AreEqual(result[0].Handlers[1], result[0].Handlers[1].Out.First().FromHandler);
            Assert.AreEqual(result[0].Handlers[2], result[0].Handlers[1].Out.First().ToHandler);
        }
    }
}