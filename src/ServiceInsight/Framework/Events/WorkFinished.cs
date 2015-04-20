namespace Particular.ServiceInsight.Desktop.Framework.Events
{
    public class WorkFinished
    {
        public WorkFinished()
            : this("Done")
        {
        }

        public WorkFinished(string format, params object[] args)
            : this(string.Format(format, args))
        {
        }

        public WorkFinished(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
}