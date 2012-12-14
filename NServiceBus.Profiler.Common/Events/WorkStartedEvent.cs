namespace NServiceBus.Profiler.Common.Events
{
    public class WorkStartedEvent
    {
        public WorkStartedEvent() 
            : this("Wait...")
        {
        }

        public WorkStartedEvent(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
}