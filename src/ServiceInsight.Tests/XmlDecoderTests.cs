namespace ServiceInsight.Tests
{
    using System.Text;
    using System.Xml;
    using NUnit.Framework;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.MessageDecoders;
    using Shouldly;

    [TestFixture]
    public class XmlDecoderTests
    {
        byte[] xmlContent;
        IContentDecoder decoder;

        [SetUp]
        public void TestInitialize()
        {
            xmlContent = Encoding.Default.GetBytes("<xml version=\"1.0\" encoding=\"utf-8\"><packages>test package</packages>");
            decoder = new XmlContentDecoder();
        }

        [Test]
        public void Should_decode_content_to_xmldocument()
        {
            decoder.Decode(xmlContent);
        }

        [Test]
        public void Should_not_get_null_document_when_decoding_flat_strings()
        {
            decoder.Decode(new byte[0]).ShouldNotBe(null);
        }       
        
        [Test]
        public void TestUpdater()
        {
            var u = new UpdateStatusCheck();

            //var res = u.CheckTheLatestVersion().ConfigureAwait(false);
            var res = u.CheckTheLatestVersion();
        }

        [Test]
        public void Should_get_empty_document_when_decoding_flat_strings()
        {
            ((XmlDocument)decoder.Decode(new byte[0]).Value).InnerXml.ShouldBeEmpty();
        }
    }
}