namespace ServiceInsight.Tests
{
    using NUnit.Framework;
    using ServiceInsight.Tests.ConversationsData;

    [TestFixture]
    class SequenceDiagramModelCreatorTestsForJson1 : SequenceDiagramModelCreatorTestsFromJson
    {
        public SequenceDiagramModelCreatorTestsForJson1()
            : base("json1.json")
        {
        }

        [Test]
        public void FirstEndpointShouldHave1Handler()
        {
            Assert.That(result[0].Handlers, Has.Count.EqualTo(1));
        }

        [Test]
        public void EndpointsShouldBeVersionedCorrectly()
        {
            Assert.Multiple(() =>
            {
                Assert.That(result[0].Handlers, Has.Count.EqualTo(1));
                Assert.That(result[0].Version, Is.EqualTo("5.2.3"));
                Assert.That(result[1].Version, Is.EqualTo("5.2.3"));
                Assert.That(result[2].Version, Is.EqualTo(null));
            });
        }

        [Test]
        public void EndpointsAreDeDup()
        {
            Assert.That(result, Has.Count.EqualTo(3));
        }
    }
}