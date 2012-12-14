using System.Text;
using System.Xml;

namespace NServiceBus.Profiler.Core.MessageDecoders
{
    public class XmlMessageDecoder : IMessageDecoder<XmlDocument>
    {
        public XmlDocument Decode(byte[] content)
        {
            var doc = new XmlDocument();

            try
            {
                var xml = Encoding.UTF8.GetString(content);
                doc.LoadXml(xml);
            }
            catch(XmlException)
            {
            }

            return doc;
        }

        object IMessageDecoder.Decode(byte[] content)
        {
            return Decode(content);
        }
    }
}