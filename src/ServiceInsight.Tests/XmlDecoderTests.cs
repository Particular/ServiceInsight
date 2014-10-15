namespace Particular.ServiceInsight.Tests
{
    using System.Text;
    using System.Xml;
    using NUnit.Framework;
    using Particular.ServiceInsight.Desktop.Framework.MessageDecoders;
    using Shouldly;

    [TestFixture]
    public class XmlDecoderTests
    {
        byte[] XmlContent;
        IContentDecoder Decoder;

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