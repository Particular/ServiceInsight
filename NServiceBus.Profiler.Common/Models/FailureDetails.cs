using System;

namespace NServiceBus.Profiler.Common.Models
{
    public class FailureDetails
    {
        public int NumberOfTimesFailed { get; set; }
        public DateTime TimeOfFailure { get; set; }
        public DateTime ResolvedAt { get; set; }
    }
}