namespace NServiceBus.Profiler.Core.MessageDecoders
{
    public interface IMessageDecoder<out T> : IMessageDecoder
    {
        new T Decode(byte[] content);
    }

    public interface IMessageDecoder
    {
        object Decode(byte[] content);
    }
}