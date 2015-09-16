namespace Particular.ServiceInsight.Tests
{
    using NUnit.Framework;
    using Particular.ServiceInsight.Tests.ConversationsData;

    [TestFixture]
    class SequenceDiagramModelCreatorTestsForJson3 : SequenceDiagramModelCreatorTestsFromJson
    {
        public SequenceDiagramModelCreatorTestsForJson3() : base("json3.json")
        {

        }

        [Test]
        public void EndpointShouldHave4Handler()
        {
            Assert.AreEqual(7, result.Count);
            Assert.AreEqual("System.Web", result[0].Name);
            Assert.AreEqual("Particular.ApplicationProcessor.LoanProcessingSagaHandler", result[1].Name);
        }
    }
}