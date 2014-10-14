namespace Particular.ServiceInsight.Desktop.Models
{
    using System;
    using Humanizer;

    public class MessageStatistics
    {
        public TimeSpan CriticalTime { get; set; }
        public TimeSpan ProcessingTime { get; set; }
        public TimeSpan DeliveryTime { get; set; }

        public string ElapsedCriticalTime
        {
            get { return CriticalTime.Humanize(); }
        }

        public string ElapsedProcessingTime
        {
            get { return ProcessingTime.Humanize(); }
        }

        public string ElapsedDeliveryTime
        {
            get { return DeliveryTime.Humanize(); }
        }
    }
}