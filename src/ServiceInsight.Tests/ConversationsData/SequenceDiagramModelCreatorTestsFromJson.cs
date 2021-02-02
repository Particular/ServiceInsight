namespace ServiceInsight.Tests.ConversationsData
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using NSubstitute;
    using NUnit.Framework;
    using ServiceInsight.Framework.MessageDecoders;
    using ServiceInsight.Models;
    using ServiceInsight.SequenceDiagram;
    using ServiceInsight.SequenceDiagram.Diagram;

    abstract class SequenceDiagramModelCreatorTestsFromJson
    {
        protected ReadOnlyCollection<EndpointItem> result;

        protected SequenceDiagramModelCreatorTestsFromJson(string fileName)
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\ConversationsData\" + fileName);
            var content = File.ReadAllText(path);
            var deserializer = new JsonMessageDeserializer
            {
                DateFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK"
            };

            var messages = deserializer.Deserialize<List<StoredMessage>>(new PayLoad(content));
            ModelCreator = new ModelCreator(messages, GetContainer());

            result = ModelCreator.Endpoints;
        }

        protected ModelCreator ModelCreator { get; }

        IMessageCommandContainer GetContainer() => Substitute.For<IMessageCommandContainer>();
    }
}