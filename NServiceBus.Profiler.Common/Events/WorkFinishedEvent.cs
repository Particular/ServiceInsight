namespace NServiceBus.Profiler.Common.Events
{
    public class WorkFinishedEvent
    {
        public WorkFinishedEvent()
            : this("Done")
        {
        }

        public WorkFinishedEvent(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
}