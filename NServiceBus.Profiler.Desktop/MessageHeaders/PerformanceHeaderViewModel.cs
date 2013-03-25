using System;
using System.Collections.Generic;
using System.Windows.Media;
using Caliburn.PresentationFramework.ApplicationModel;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core;
using NServiceBus.Profiler.Core.MessageDecoders;
using NServiceBus.Profiler.Desktop.Properties;
using NServiceBus.Profiler.Common.ExtensionMethods;

namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    public class PerformanceHeaderViewModel : HeaderInfoViewModelBase
    {
        public PerformanceHeaderViewModel(
            IEventAggregator eventAggregator, 
            IContentDecoder<IList<HeaderInfo>> decoder, 
            IQueueManagerAsync queueManager) 
            : base(eventAggregator, decoder, queueManager)
        {
            DisplayName = "Performance";
        }

        public override ImageSource GroupImage
        {
            get { return Resources.HeaderPerformance.ToBitmapImage(); }
        }

        protected override bool IsMatchingHeader(HeaderInfo header)
        {
            //TODO: Add a diff between processing started and ended
            return header.Key.EndsWith("TimeSent", StringComparison.OrdinalIgnoreCase)          ||
                   header.Key.EndsWith("ProcessingStarted", StringComparison.OrdinalIgnoreCase) ||
                   header.Key.EndsWith("ProcessingEnded", StringComparison.OrdinalIgnoreCase);
        }
    }
}