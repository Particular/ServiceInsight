namespace ServiceInsight.Tests
{
    using System.Linq;
    using ConversationsData;
    using NUnit.Framework;

    [TestFixture]
    class SequenceDiagramModelCreatorTestsForJson5 : SequenceDiagramModelCreatorTestsFromJson
    {
        public SequenceDiagramModelCreatorTestsForJson5()
            : base("json5.json")
        {
        }

        [Test]
        public void ShouldHaveStartOfConversationAsFirstHandler()
        {
            Assert.That(ModelCreator.Handlers[0].ID, Is.EqualTo(SequenceDiagram.ModelCreator.ConversationStartHandlerName));
            Assert.That(ModelCreator.Handlers[0].Name, Is.EqualTo(null));
        }

        [Test]
        public void StartShouldHappenBeforeCompleted()
        {
            var startHandler = ModelCreator.Handlers.Single(h => h.Name == "RelocationProcessStartedNotification");
            var endHandler = ModelCreator.Handlers.Single(h => h.Name == "RelocationProcessCompletedNotification");

            Assert.Less(ModelCreator.Handlers.IndexOf(startHandler), ModelCreator.Handlers.IndexOf(endHandler));
        }
    }
}