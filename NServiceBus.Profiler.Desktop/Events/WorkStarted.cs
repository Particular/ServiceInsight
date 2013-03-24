namespace NServiceBus.Profiler.Desktop.Events
{
    public class WorkStarted
    {
        public WorkStarted() 
            : this("Wait...")
        {
        }

        public WorkStarted(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
}