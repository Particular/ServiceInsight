namespace Particular.ServiceInsight.Desktop.Models
{
    using System;

    public class MessageStatistics
    {
        public TimeSpan ProcessingTime { get; set; }
        
        public string ElapsedProcessingTime
        {
            get { return ProcessingTime.SubmillisecondHumanize(); }
        }
    }
}