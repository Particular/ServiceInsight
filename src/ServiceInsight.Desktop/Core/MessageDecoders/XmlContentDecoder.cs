using System;
using System.Text;
using System.Xml;

namespace NServiceBus.Profiler.Desktop.Core.MessageDecoders
{
    public class XmlContentDecoder : IContentDecoder<XmlDocument>
    {
        public DecoderResult<XmlDocument> Decode(byte[] content)
        {
            var doc = new XmlDocument();

            if (content != null && content.Length > 0)
            {
                var xml = Encoding.UTF8.GetString(content);
                if (TryLoadIntoDocument(xml, doc))
                {
                    return new DecoderResult<XmlDocument>(doc);
                }
                else
                {
                    //TODO: Issues RESTSharp deserializer when reading byte array as string
                    xml = GetFromBase64String(content);
                    if (TryLoadIntoDocument(xml, doc))
                    {
                        return new DecoderResult<XmlDocument>(doc);
                    }
                }
            }

            return new DecoderResult<XmlDocument>(doc, false);
        }

        private static string GetFromBase64String(byte[] content)
        {
            try
            {
                var base64EncodedString = Encoding.UTF8.GetString(content);
                var encodedDataAsBytes = Convert.FromBase64String(base64EncodedString);
                return Encoding.UTF8.GetString(encodedDataAsBytes);
            }
            catch
            {
                return string.Empty;
            }
        }

        private bool TryLoadIntoDocument(string xml, XmlDocument document)
        {
            if (string.IsNullOrWhiteSpace(xml)) return false;

            try
            {
                document.LoadXml(xml);
                return true;
            }
            catch
            {
                return false;
            }
        }

        DecoderResult IContentDecoder.Decode(byte[] content)
        {
            return Decode(content);
        }
    }
}