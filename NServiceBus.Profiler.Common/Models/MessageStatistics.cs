using System;

namespace NServiceBus.Profiler.Common.Models
{
    public class MessageStatistics
    {
        public TimeSpan CriticalTime { get; set; }
        public TimeSpan ProcessingTime { get; set; }
    }
}