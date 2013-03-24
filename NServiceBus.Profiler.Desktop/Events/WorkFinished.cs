namespace NServiceBus.Profiler.Desktop.Events
{
    public class WorkFinished
    {
        public WorkFinished()
            : this("Done")
        {
        }

        public WorkFinished(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
}