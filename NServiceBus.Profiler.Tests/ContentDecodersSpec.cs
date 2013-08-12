using System.Text;
using System.Xml;
using Machine.Specifications;
using Particular.ServiceInsight.Desktop.Core.MessageDecoders;

namespace NServiceBus.Profiler.Tests.Decoders
{
    [Subject("content decoders")]
    public class when_decoding_string_conent
    {
        protected static byte[] StringContent;
        protected static string Decoded;
        protected static IContentDecoder Decoder;

        Establish context = () =>
        {
            StringContent = Encoding.UTF8.GetBytes("This is a string content");
        };

        Because of = () => Decoder = new StringContentDecoder();

        It should_be_able_to_decode_conent_to_string = () => Decoder.Decode(StringContent).IsParsed.ShouldEqual(true);
        It should_decode_conent_to_string = () => Decoder.Decode(StringContent).Value.ShouldEqual("This is a string content");
        It should_return_empty_string_if_there_is_no_content = () => Decoder.Decode(new byte[0]).Value.ShouldEqual("");
        It should_return_empty_string_if_content_is_null = () => Decoder.Decode(null).IsParsed.ShouldEqual(false);
    }

    [Subject("content decoders")]
    public class when_decoding_xml_content
    {
        protected static XmlDocument Decoded;
        protected static byte[] XmlContent;
        protected static IContentDecoder Decoder;

        Establish context = () =>
        {
            XmlContent = Encoding.Default.GetBytes("<xml version=\"1.0\" encoding=\"utf-8\"><packages>test package</packages>");
        };

        Because of = () => Decoder = new XmlContentDecoder();

        It should_decode_content_to_xmldocument = () => Decoder.Decode(XmlContent);
        It should_not_get_null_document_when_decoding_flat_strings = () => Decoder.Decode(new byte[0]).ShouldNotBeNull();
        It should_get_empty_document_when_decoding_flat_strings = () => ((XmlDocument)Decoder.Decode(new byte[0]).Value).InnerXml.ShouldBeEmpty();
    }
}