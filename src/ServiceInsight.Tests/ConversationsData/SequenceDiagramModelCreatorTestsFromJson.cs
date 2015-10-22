namespace Particular.ServiceInsight.Tests.ConversationsData
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using Autofac;
    using global::ServiceInsight.SequenceDiagram;
    using global::ServiceInsight.SequenceDiagram.Diagram;
    using Particular.ServiceInsight.Desktop.Framework.Commands;
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
            var creator = new ModelCreator(messages, GetContainer());

            result = creator.Endpoints;
        }

        private IContainer GetContainer()
        {
            var builder = new ContainerBuilder();

            //TODO: Yuck! Too much coupling. Why do we need an IContainer for Arrow? Can we use a Func<T> instead?
            builder.RegisterType<CopyConversationIDCommand>();
            builder.RegisterType<CopyMessageURICommand>();
            builder.RegisterType<RetryMessageCommand>();
            builder.RegisterType<SearchByMessageIDCommand>();

            return builder.Build();
        }
    }
}