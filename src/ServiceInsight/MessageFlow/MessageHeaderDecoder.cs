namespace ServiceInsight.MessageFlow
{
    using System.Collections.Generic;
    using System.Text;
    using Models;
    using ServiceInsight.Framework.MessageDecoders;

    public class MessageHeaderDecoder
    {
        public MessageHeaderDecoder(IContentDecoder<IList<HeaderInfo>> decoder, MessageBody message)
        {
            RawHeader = Encoding.UTF8.GetString(message.HeaderRaw);
            var decodedResult = decoder.Decode(message.HeaderRaw);
            DecodedHeaders = decodedResult.IsParsed ? decodedResult.Value : new HeaderInfo[0];
        }

        public IList<HeaderInfo> DecodedHeaders { get; private set; }
        public string RawHeader { get; private set; }
    }
}