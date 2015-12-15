namespace ServiceInsight.Framework.MessageDecoders
{
    using System;
    using System.Text;
    using Anotar.Serilog;

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
                catch (Exception ex)
                {
                    LogTo.Error(ex, "Error trying to decode string {content}", content);
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