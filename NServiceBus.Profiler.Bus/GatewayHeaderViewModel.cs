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
    public class GatewayHeaderViewModel : HeaderInfoViewModelBase
    {
        public GatewayHeaderViewModel(
            IEventAggregator eventAggregator, 
            IMessageDecoder<string> decoder, 
            IQueueManagerAsync queueManager, 
            IClipboard clipboard) : base(eventAggregator, decoder, queueManager, clipboard)
        {
            DisplayName = "Gateway";
        }

        protected override bool IsMatchingHeader(HeaderInfo header)
        {
            return header.Key.EndsWith(".From", StringComparison.OrdinalIgnoreCase)             ||
                   header.Key.EndsWith(".To", StringComparison.OrdinalIgnoreCase)               ||
                   header.Key.EndsWith(".DestinationSites", StringComparison.OrdinalIgnoreCase) ||
                   header.Key.EndsWith(".OriginatingSite", StringComparison.OrdinalIgnoreCase)  ||
                   header.Key.EndsWith(".Header.RouteTo", StringComparison.OrdinalIgnoreCase);
        }
    }
}