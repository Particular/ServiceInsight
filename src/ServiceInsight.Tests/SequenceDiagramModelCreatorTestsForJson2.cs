namespace ServiceInsight.Tests
{
    using System.Linq;
    using NUnit.Framework;
    using ServiceInsight.Tests.ConversationsData;

    [TestFixture]
    class SequenceDiagramModelCreatorTestsForJson2 : SequenceDiagramModelCreatorTestsFromJson
    {
        public SequenceDiagramModelCreatorTestsForJson2()
            : base("json2.json")
        {
        }

        [Test]
        public void EndpointShouldHave4Handler()
        {
            Assert.That(result[0].Handlers, Has.Count.EqualTo(4));
            Assert.Multiple(() =>
            {
                Assert.That(result[0].Handlers[0].Name, Is.EqualTo(null));
                Assert.That(result[0].Handlers[1].Name, Is.EqualTo("StartOrder"));
                Assert.That(result[0].Handlers[2].Name, Is.EqualTo("CoolingOff"));
                Assert.That(result[0].Handlers[3].Name, Is.EqualTo("CompleteOrder"));
            });
        }

        [Test]
        public void ArrowsHaveFromAndToPopulated()
        {
            Assert.Multiple(() =>
            {
                Assert.That(result[0].Handlers[0].In, Is.Null);
                Assert.That(result[0].Handlers[0].Out.First().FromHandler, Is.EqualTo(result[0].Handlers[0]));
                Assert.That(result[0].Handlers[0].Out.First().ToHandler, Is.EqualTo(result[0].Handlers[1]));

                Assert.That(result[0].Handlers[1].In.FromHandler, Is.EqualTo(result[0].Handlers[0]));
                Assert.That(result[0].Handlers[1].In.ToHandler, Is.EqualTo(result[0].Handlers[1]));
                Assert.That(result[0].Handlers[1].Out.First().FromHandler, Is.EqualTo(result[0].Handlers[1]));
                Assert.That(result[0].Handlers[1].Out.First().ToHandler, Is.EqualTo(result[0].Handlers[2]));
            });
        }
    }
}