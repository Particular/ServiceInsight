namespace ServiceInsight.Tests
{
    using NUnit.Framework;
    using ServiceInsight.Tests.ConversationsData;

    [TestFixture]
    class SequenceDiagramModelCreatorTestsForJson3 : SequenceDiagramModelCreatorTestsFromJson
    {
        public SequenceDiagramModelCreatorTestsForJson3()
            : base("json3.json")
        {
        }

        [Test]
        public void EndpointShouldHave4Handler()
        {
            Assert.That(result.Count, Is.EqualTo(7));
            Assert.That(result[0].Name, Is.EqualTo("System.Web"));
            Assert.That(result[1].Name, Is.EqualTo("Particular.ApplicationProcessor.LoanProcessingSagaHandler"));
            Assert.That(result[2].Name, Is.EqualTo("Particular.AnalyticsDatabase.AnalyticsDataHandler"));
            Assert.That(result[3].Name, Is.EqualTo("Particular.ApplicationProcessor.LoanDecisionHandler"));
            Assert.That(result[4].Name, Is.EqualTo("Particular.ApplicationProcessor.PartnerDecisionPostbackHandler"));
            Assert.That(result[5].Name, Is.EqualTo("Particular.ApplicationProcessor.WebLoanDecisionUpdateHandler"));
            Assert.That(result[6].Name, Is.EqualTo("Particular.ApplicationProcessor.EmailHandler"));
        }
    }
}