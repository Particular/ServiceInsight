using System;
using System.Collections.Generic;
using System.Windows.Media;
using Caliburn.PresentationFramework.ApplicationModel;
using ExceptionHandler;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core;
using NServiceBus.Profiler.Core.MessageDecoders;
using NServiceBus.Profiler.Desktop.Properties;
using NServiceBus.Profiler.Common.ExtensionMethods;

namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    public class GatewayHeaderViewModel : HeaderInfoViewModelBase
    {
        public GatewayHeaderViewModel(
            IEventAggregator eventAggregator, 
            IContentDecoder<IList<HeaderInfo>> decoder, 
            IQueueManagerAsync queueManager, 
            IClipboard clipboard) 
            : base(eventAggregator, decoder, queueManager, clipboard)
        {
            DisplayName = "Gateway";
        }

        public override ImageSource GroupImage
        {
            get { return Resources.HeaderGateway.ToBitmapImage(); }
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