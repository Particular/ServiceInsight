namespace NServiceBus.Profiler.Desktop.Events
{
    public class AsyncOperationFailedEvent
    {
        public int ErrorCode { get; set; }
        public string Description { get; set; }
    }
}