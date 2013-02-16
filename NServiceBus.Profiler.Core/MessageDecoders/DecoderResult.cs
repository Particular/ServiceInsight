namespace NServiceBus.Profiler.Core.MessageDecoders
{
    public abstract class DecoderResult
    {
        public bool IsParsed { get; protected set; }
        public object Value { get; protected set; }
    }

    public class DecoderResult<T> : DecoderResult
    {
        public DecoderResult()
        {
            Value = default(T);
            IsParsed = false;
        }

        public DecoderResult(T value, bool isParsed = true)
        {
            Value = value;
            IsParsed = isParsed;
        }

        public new T Value { get; set; }
    }
}