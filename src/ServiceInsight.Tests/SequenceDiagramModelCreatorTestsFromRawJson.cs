namespace Particular.ServiceInsight.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using global::ServiceInsight.SequenceDiagram;
    using global::ServiceInsight.SequenceDiagram.Diagram;
    using NUnit.Framework;
    using Particular.ServiceInsight.Desktop.Framework.MessageDecoders;
    using Particular.ServiceInsight.Desktop.SequenceDiagram;
    using RestSharp;

    [TestFixture]
    class SequenceDiagramModelCreatorTestsFromRawJson
    {
        List<EndpointItem> result;

        public SequenceDiagramModelCreatorTestsFromRawJson()
        {
            var content = File.ReadAllText(@"..\..\ConversationsData\json1.json");
            var deserializer = new JsonMessageDeserializer
            {
                DateFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK"
            };
            var messages = deserializer.Deserialize<List<ReceivedMessage>>(new PayLoad(content));

            var creator = new ModelCreator(messages);
            result = creator.GetModel().OfType<EndpointItem>().ToList();
        }

        [Test]
        public void FirstEndpointShouldHave1Handler()
        {
            Assert.AreEqual(1, result[0].Handlers.Count);
            Assert.AreEqual("5.2.3", result[0].Version);
            Assert.AreEqual("5.2.3", result[1].Version);
            Assert.AreEqual(null, result[2].Version);
        }

        [Test]
        public void EndpointsShouldBeVersionedCorrectly()
        {
            Assert.AreEqual(1, result[0].Handlers.Count);
            Assert.AreEqual("5.2.3", result[0].Version);
            Assert.AreEqual("5.2.3", result[1].Version);
            Assert.AreEqual(null, result[2].Version);
        }

        class PayLoad : IRestResponse
        {
            public PayLoad(string content)
            {
                Content = content;
            }

            public IRestRequest Request { get; set; }
            public string ContentType { get; set; }
            public long ContentLength { get; set; }
            public string ContentEncoding { get; set; }
            public string Content { get; set; }
            public HttpStatusCode StatusCode { get; set; }
            public string StatusDescription { get; set; }
            public byte[] RawBytes { get; set; }
            public Uri ResponseUri { get; set; }
            public string Server { get; set; }
            public IList<RestResponseCookie> Cookies { get; private set; }
            public IList<Parameter> Headers { get; private set; }
            public ResponseStatus ResponseStatus { get; set; }
            public string ErrorMessage { get; set; }
            public Exception ErrorException { get; set; }
        }
    }
}