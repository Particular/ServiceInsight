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
            Assert.AreEqual(SequenceDiagram.ModelCreator.ConversationStartHandlerName, ModelCreator.Handlers[0].ID);
            Assert.AreEqual(null, ModelCreator.Handlers[0].Name);
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