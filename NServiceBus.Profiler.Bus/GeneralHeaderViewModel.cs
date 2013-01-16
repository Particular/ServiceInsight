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
    public class GeneralHeaderViewModel : HeaderInfoViewModelBase
    {
        public GeneralHeaderViewModel(
            IEventAggregator eventAggregator, 
            IMessageDecoder<string> decoder, 
            IQueueManagerAsync queueManager, 
            IClipboard clipboard) 
            : base(eventAggregator, decoder, queueManager, clipboard)
        {
            DisplayName = "General";
        }

        protected override bool IsMatchingHeader(HeaderInfo header)
        {
            return !header.Key.StartsWith("NServiceBus", StringComparison.OrdinalIgnoreCase) ||
                   header.Key.EndsWith("Version", StringComparison.OrdinalIgnoreCase) ||
                   header.Key.EndsWith("EnclosedMessageTypes", StringComparison.OrdinalIgnoreCase);
        }
    }
}