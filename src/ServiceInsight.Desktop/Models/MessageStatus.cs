using System.ComponentModel;

namespace NServiceBus.Profiler.Desktop.Models
{
    public enum MessageStatus
    {
        [Description("Failed")]
        Failed = 1,
        
        [Description("Repeated Failures")]
        RepeatedFailure = 2,

        [Description("Successful")]
        Successful = 3,

        [Description("Successfully resolved")]
        ResolvedSuccessfully = 4,

        [Description("Failure message archived")]
        ArchivedFailure = 5,

        [Description("Retry Requested")]
        RetryIssued = 6
    }
}