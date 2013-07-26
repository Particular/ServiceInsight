using System;
using System.Collections.Generic;
using System.ComponentModel;
using Caliburn.PresentationFramework.ApplicationModel;
using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.Core.MessageDecoders;
using NServiceBus.Profiler.Desktop.ExtensionMethods;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    [TypeConverter(typeof(HeaderInfoTypeConverter))]
    public class PerformanceHeaderViewModel : HeaderInfoViewModelBase, IPerformanceHeaderViewModel
    {
        public PerformanceHeaderViewModel(
            IEventAggregator eventAggregator, 
            IContentDecoder<IList<HeaderInfo>> decoder, 
            IQueueManagerAsync queueManager) 
            : base(eventAggregator, decoder, queueManager)
        {
            DisplayName = "Performance";
        }

        [Description("Time the message was sent")]
        public DateTime? TimeSent { get; set; }

        [Description("Time the processing started")]
        public DateTime? ProcessingStarted { get; set; }

        [Description("Time the processing finished")]
        public DateTime? ProcessingEnded { get; set; }

        [Description("Calculated duration of processing")]
        public string ProcessingTime { get; set; }

        [Description("Calculated time from the sending of the message by the sending endpoint, until message processing started in the receiving endpoint")]
        public string DeliveryTime { get; set; }

        [Description("Calculated time from the sending of the message by the sending endpoint, until message processing completed in the receiving endpoint")]
        public string CriticalTime { get; set; }

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