using System.Text;

namespace NServiceBus.Profiler.Core.MessageDecoders
{
    public class StringMessageDecoder : IMessageDecoder<string>
    {
        public string Decode(byte[] content)
        {
            if (content == null) return null;
            return Encoding.UTF8.GetString(content);
        }

        object IMessageDecoder.Decode(byte[] content)
        {
            return Decode(content);
        }
    }
}