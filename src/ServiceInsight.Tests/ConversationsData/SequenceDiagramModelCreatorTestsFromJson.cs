namespace Particular.ServiceInsight.Tests.ConversationsData
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using global::ServiceInsight.SequenceDiagram;
    using global::ServiceInsight.SequenceDiagram.Diagram;
    using Particular.ServiceInsight.Desktop.Framework.MessageDecoders;

    abstract class SequenceDiagramModelCreatorTestsFromJson
    {
        protected ReadOnlyCollection<EndpointItem> result;

        protected SequenceDiagramModelCreatorTestsFromJson(string fileName)
        {
            var content = File.ReadAllText(@"..\..\ConversationsData\" + fileName);
            var deserializer = new JsonMessageDeserializer
            {
                DateFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK"
            };
            var messages = deserializer.Deserialize<List<ReceivedMessage>>(new PayLoad(content));

            var creator = new ModelCreator(messages);
            result = creator.Endpoints;
        }
    }
}