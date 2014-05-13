namespace Particular.ServiceInsight.Desktop.MessageProperties
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Core.MessageDecoders;
    using ExtensionMethods;
    using Models;

    public class PerformanceHeaderViewModel : HeaderInfoViewModelBase, IPerformanceHeaderViewModel
    {
        public PerformanceHeaderViewModel(
            IEventAggregator eventAggregator, 
            IContentDecoder<IList<HeaderInfo>> decoder) 
            : base(eventAggregator, decoder)
        {
            DisplayName = "Performance";
        }

        [Description("Time the message was sent")]
        public DateTime? TimeSent { get; private set; }

        [Description("Time the processing started")]
        public DateTime? ProcessingStarted { get; private set; }

        [Description("Time the processing finished")]
        public DateTime? ProcessingEnded { get; private set; }

        [Description("Calculated duration of processing")]
        public string ProcessingTime { get; private set; }

        [Description("Calculated time from the sending of the message by the sending endpoint, until message processing started in the receiving endpoint")]
        public string DeliveryTime { get; private set; }

        [Description("Calculated time from the sending of the message by the sending endpoint, until message processing completed in the receiving endpoint")]
        public string CriticalTime { get; private set; }

        protected override void OnMessagePropertiesLoaded()
        {
            if (!ProcessingStarted.HasValue ||
                !ProcessingEnded.HasValue ||
                !TimeSent.HasValue)
            {
                return;
            }

            ProcessingTime = (ProcessingEnded.Value - ProcessingStarted.Value).GetElapsedTime();
            CriticalTime = (ProcessingEnded.Value - TimeSent.Value).GetElapsedTime();
            DeliveryTime = (ProcessingStarted.Value - TimeSent.Value).GetElapsedTime();
        }

        protected override void MapHeaderKeys()
        {
            ConditionsMap.Add(h => h.Key.EndsWith("TimeSent", StringComparison.OrdinalIgnoreCase), h => TimeSent = h.Value.ParseHeaderDate());
            ConditionsMap.Add(h => h.Key.EndsWith("ProcessingStarted", StringComparison.OrdinalIgnoreCase), h => ProcessingStarted = h.Value.ParseHeaderDate());
            ConditionsMap.Add(h => h.Key.EndsWith("ProcessingEnded", StringComparison.OrdinalIgnoreCase), h => ProcessingEnded = h.Value.ParseHeaderDate());
        }

        protected override void ClearHeaderValues()
        {
            TimeSent = null;
            ProcessingStarted = null;
            ProcessingEnded = null;
            ProcessingTime = null;
            DeliveryTime = null;
            CriticalTime = null;
        }
    }
}