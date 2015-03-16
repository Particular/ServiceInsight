namespace Particular.ServiceInsight.Desktop.Models
{
    using System;

    public class MessageStatistics
    {
        public TimeSpan CriticalTime { get; set; }
        public TimeSpan ProcessingTime { get; set; }
        public TimeSpan DeliveryTime { get; set; }

        public string ElapsedCriticalTime
        {
            get { return CriticalTime.SubmillisecondHumanize(); }
        }

        public string ElapsedProcessingTime
        {
            get { return ProcessingTime.SubmillisecondHumanize(); }
        }

        public string ElapsedDeliveryTime
        {
            get { return DeliveryTime.SubmillisecondHumanize(); }
        }
    }
}