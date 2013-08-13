namespace Particular.ServiceInsight.Desktop.Core.MessageDecoders
{
    public interface IContentDecoder<T> : IContentDecoder
    {
        new DecoderResult<T> Decode(byte[] content);
    }

    public interface IContentDecoder
    {
        DecoderResult Decode(byte[] content);
    }
}