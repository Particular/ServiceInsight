using System;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Views;
using ExceptionHandler;
using NServiceBus.Profiler.Core;
using NServiceBus.Profiler.Core.MessageDecoders;

namespace NServiceBus.Profiler.Bus
{
    [View(typeof(HeaderInfoView))]
    public class PerformanceHeaderViewModel : HeaderInfoViewModelBase
    {
        public PerformanceHeaderViewModel(
            IEventAggregator eventAggregator, 
            IMessageDecoder<string> decoder, 
            IQueueManagerAsync queueManager, 
            IClipboard clipboard) : base(eventAggregator, decoder, queueManager, clipboard)
        {
            DisplayName = "Performance";
        }

        protected override bool IsMatchingHeader(HeaderInfo header)
        {
            return header.Key.EndsWith("TimeSent", StringComparison.OrdinalIgnoreCase) ||
                   header.Key.EndsWith("TimeOfFailure", StringComparison.OrdinalIgnoreCase); //TODO: Add processing Start and End date
        }
    }
}