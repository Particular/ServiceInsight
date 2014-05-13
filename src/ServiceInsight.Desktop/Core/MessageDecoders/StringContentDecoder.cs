namespace Particular.ServiceInsight.Desktop.Core.MessageDecoders
{
    using System.Text;

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
// ReSharper disable EmptyGeneralCatchClause
                catch
// ReSharper restore EmptyGeneralCatchClause
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