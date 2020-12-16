namespace ServiceInsight.Models
{
    using System.ComponentModel;

    public enum MessageStatus
    {
        [Description("Failed")]
        Failed = 1,

        [Description("Repeated failures")]
        RepeatedFailure = 2,

        [Description("Successful")]
        Successful = 3,

        [Description("Successful after retries")]
        ResolvedSuccessfully = 4,

        [Description("Failure message archived")]
        ArchivedFailure = 5,

        [Description("Retry requested")]
        RetryIssued = 6
    }
}
