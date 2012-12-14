using System.Text;
using System.Xml;
using Machine.Specifications;
using NServiceBus.Profiler.Core.MessageDecoders;

namespace NServiceBus.Profiler.Tests.Decoders
{
    [Subject("content decoders")]
    public abstract class with_message_decoders
    {
        protected static byte[] StringContent;
        protected static byte[] XmlContent;
        protected static IMessageDecoder Decoder;
        
        Establish context = () =>
        {
            StringContent = Encoding.Default.GetBytes("This is a string content");
            XmlContent = Encoding.Default.GetBytes("<xml version=\"1.0\" encoding=\"utf-8\"><packages>test package</packages>");
        };
    }

    [Subject("content decoders")]
    public class when_decoding_string_conent : with_message_decoders
    {
        protected static string Decoded;

        Because of = () => Decoder = new StringMessageDecoder();

        It should_decode_conent_to_string = () => Decoder.Decode(StringContent).ShouldEqual("This is a string content");
        It should_return_empty_string_if_there_is_no_content = () => Decoder.Decode(new byte[0]).ShouldEqual("");
        It should_return_null_string_if_content_is_null = () => Decoder.Decode(null).ShouldEqual(null);
    }

    [Subject("content decoders")]
    public class when_decoding_xml_content : with_message_decoders
    {
        protected static XmlDocument Decoded;

        Because of = () => Decoder = new XmlMessageDecoder();

        It should_decode_content_to_xmldocument = () => Decoder.Decode(XmlContent);
        It should_not_get_null_document_when_decoding_flat_strings = () => Decoder.Decode(StringContent).ShouldNotBeNull();
        It should_get_empty_document_when_decoding_flat_strings = () => ((XmlDocument)Decoder.Decode(StringContent)).InnerXml.ShouldBeEmpty();
    }
}