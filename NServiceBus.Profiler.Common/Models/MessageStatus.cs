namespace NServiceBus.Profiler.Common.Models
{
    public enum MessageStatus
    {
        Failed = 1,
        RepeatedFailures = 2,
        Successfull = 3,
        RetryIssued = 4
    }
}