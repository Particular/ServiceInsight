using System.Text;

namespace Particular.ServiceInsight.Desktop.Core.MessageDecoders
{
    public class StringContentDecoder : IContentDecoder<string>
    {
        public DecoderResult<string> Decode(byte[] content)
        {
            if (content != null && content.Length > 0)
            {
                try
                {
                    return new DecoderResult<string>(Encoding.UTF8.GetString(content));
                }
                catch
                {
                }
            }

            return new DecoderResult<string>("", false);
        }

        DecoderResult IContentDecoder.Decode(byte[] content)
        {
            return Decode(content);
        }
    }
}