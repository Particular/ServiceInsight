using System;

namespace NServiceBus.Profiler.Desktop.Models
{
    public class FailureDetails
    {
        public int NumberOfTimesFailed { get; set; }
        public DateTime TimeOfFailure { get; set; }
        public DateTime ResolvedAt { get; set; }
    }
}