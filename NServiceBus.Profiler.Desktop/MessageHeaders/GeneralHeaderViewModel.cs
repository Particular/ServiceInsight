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
    public class GeneralHeaderViewModel : HeaderInfoViewModelBase
    {
        public GeneralHeaderViewModel(
            IEventAggregator eventAggregator, 
            IContentDecoder<IList<HeaderInfo>> decoder, 
            IQueueManagerAsync queueManager, 
            IClipboard clipboard) 
            : base(eventAggregator, decoder, queueManager, clipboard)
        {
            DisplayName = "General";
        }

        public override TabPriority Order
        {
            get { return TabPriority.High; }
        }

        public override ImageSource GroupImage
        {
            get { return Resources.HeaderGeneral.ToBitmapImage(); }
        }

        protected override bool IsMatchingHeader(HeaderInfo header)
        {
            return !header.Key.StartsWith("NServiceBus", StringComparison.OrdinalIgnoreCase)       ||
                   header.Key.EndsWith("Version", StringComparison.OrdinalIgnoreCase)              ||
                   header.Key.EndsWith("EnclosedMessageTypes", StringComparison.OrdinalIgnoreCase) ||
                   header.Key.EndsWith("Retries", StringComparison.OrdinalIgnoreCase)              ||
                   header.Key.EndsWith("RelatedTo", StringComparison.OrdinalIgnoreCase)            ||
                   header.Key.EndsWith("ContentType", StringComparison.OrdinalIgnoreCase)          ||
                   header.Key.EndsWith("IsDeferedMessage", StringComparison.OrdinalIgnoreCase);
        }
    }
}