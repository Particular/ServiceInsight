using System;
using NServiceBus.Profiler.Common.ExtensionMethods;

namespace NServiceBus.Profiler.Common.Models
{
    public class MessageStatistics
    {
        public TimeSpan CriticalTime { get; set; }
        public TimeSpan ProcessingTime { get; set; }

        public string ElapsedCriticalTime
        {
            get { return CriticalTime.GetElapsedTime(); }
        } 

        public string ElapsedProcessingTime
        {
            get { return ProcessingTime.GetElapsedTime(); }
        }
    }
}