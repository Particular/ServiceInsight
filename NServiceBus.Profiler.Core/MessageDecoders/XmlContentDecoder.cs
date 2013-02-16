using System.Text;
using System.Xml;

namespace NServiceBus.Profiler.Core.MessageDecoders
{
    public class XmlContentDecoder : IContentDecoder<XmlDocument>
    {
        public DecoderResult<XmlDocument> Decode(byte[] content)
        {
            var doc = new XmlDocument();

            if (content != null && content.Length > 0)
            {
                try
                {
                    var xml = Encoding.UTF8.GetString(content);
                    doc.LoadXml(xml);
                    return new DecoderResult<XmlDocument>(doc);
                }
                catch
                {
                }
            }

            return new DecoderResult<XmlDocument>(doc, false);
        }

        DecoderResult IContentDecoder.Decode(byte[] content)
        {
            return Decode(content);
        }
    }
}