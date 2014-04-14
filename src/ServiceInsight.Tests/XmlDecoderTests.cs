using System.Text;
using System.Xml;
using NServiceBus.Profiler.Desktop.Core.MessageDecoders;
using NUnit.Framework;
using Shouldly;

namespace NServiceBus.Profiler.Tests
{
    [TestFixture]
    public class XmlDecoderTests
    {
        private byte[] XmlContent;
        private IContentDecoder Decoder;

        [SetUp]
        public void TestInitialize()
        {
            XmlContent = Encoding.Default.GetBytes("<xml version=\"1.0\" encoding=\"utf-8\"><packages>test package</packages>");
            Decoder = new XmlContentDecoder();
        }

        [Test]
        public void should_decode_content_to_xmldocument()
        {
            Decoder.Decode(XmlContent);
        }

        [Test]
        public void should_not_get_null_document_when_decoding_flat_strings()
        {
            Decoder.Decode(new byte[0]).ShouldNotBe(null);
        }

        [Test]
        public void should_get_empty_document_when_decoding_flat_strings()
        {
            ((XmlDocument)Decoder.Decode(new byte[0]).Value).InnerXml.ShouldBeEmpty();
        }
    }
}