using System.Text;

namespace NServiceBus.Profiler.Core.MessageDecoders
{
    public class StringMessageDecoder : IMessageDecoder<string>
    {
        public string Decode(byte[] content)
        {
            if (content == null) return null;
            return new UTF8Encoding(false).GetString(content);
        }

        object IMessageDecoder.Decode(byte[] content)
        {
            return Decode(content);
        }
    }
}