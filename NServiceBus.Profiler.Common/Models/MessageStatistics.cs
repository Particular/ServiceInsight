using System;

namespace NServiceBus.Profiler.Common.Models
{
    public class MessageStatistics
    {
        public TimeSpan CriticalTime { get; set; }
        public TimeSpan ProcessingTime { get; set; }

        public string CriticalTimeAsString
        {
            get
            {
                var criticalTime = CriticalTime;
                if (criticalTime.TotalSeconds < 1.0)
                    return string.Format("{0}ms", criticalTime.Milliseconds);

                if (criticalTime.TotalMinutes < 1.0)
                    return string.Format("{0}s", criticalTime.Seconds);

                if (criticalTime.TotalHours < 1.0)
                    return string.Format("{0}m {1:D2}s", criticalTime.Minutes, criticalTime.Seconds);

                return string.Format("{0}h {1:D2}m {2:D2}s", (int) criticalTime.TotalHours, criticalTime.Minutes,
                                     criticalTime.Seconds);
            }
        } 
    }
}