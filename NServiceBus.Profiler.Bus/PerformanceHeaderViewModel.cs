using System;
using System.Collections.Generic;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Views;
using ExceptionHandler;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core;
using NServiceBus.Profiler.Core.MessageDecoders;

namespace NServiceBus.Profiler.Bus
{
    [View(typeof(HeaderInfoView))]
    public class PerformanceHeaderViewModel : HeaderInfoViewModelBase
    {
        public PerformanceHeaderViewModel(
            IEventAggregator eventAggregator, 
            IContentDecoder<IList<HeaderInfo>> decoder, 
            IQueueManagerAsync queueManager, 
            IClipboard clipboard) 
            : base(eventAggregator, decoder, queueManager, clipboard)
        {
            DisplayName = "Performance";
        }

        protected override bool IsMatchingHeader(HeaderInfo header)
        {
            return header.Key.EndsWith("TimeSent", StringComparison.OrdinalIgnoreCase)          ||
                   header.Key.EndsWith("ProcessingStarted", StringComparison.OrdinalIgnoreCase) ||
                   header.Key.EndsWith("ProcessingEnded", StringComparison.OrdinalIgnoreCase);
        }
    }
}