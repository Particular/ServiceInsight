namespace ServiceInsight.Tests
{
    using NUnit.Framework;
    using ServiceInsight.Tests.ConversationsData;

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
            Assert.AreEqual("Particular.AnalyticsDatabase.AnalyticsDataHandler", result[2].Name);
            Assert.AreEqual("Particular.ApplicationProcessor.LoanDecisionHandler", result[3].Name);
            Assert.AreEqual("Particular.ApplicationProcessor.PartnerDecisionPostbackHandler", result[4].Name);
            Assert.AreEqual("Particular.ApplicationProcessor.WebLoanDecisionUpdateHandler", result[5].Name);
            Assert.AreEqual("Particular.ApplicationProcessor.EmailHandler", result[6].Name);
        }
    }
}