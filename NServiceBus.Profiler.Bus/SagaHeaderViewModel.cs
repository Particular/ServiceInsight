using System;
using System.Collections.Generic;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Views;
using ExceptionHandler;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core;
using NServiceBus.Profiler.Core.MessageDecoders;

namespace NServiceBus.Profiler.Bus
{
    [View(typeof(HeaderInfoView))]
    public class SagaHeaderViewModel : HeaderInfoViewModelBase
    {
        public SagaHeaderViewModel(
            IEventAggregator eventAggregator, 
            IContentDecoder<IList<HeaderInfo>> decoder, 
            IQueueManagerAsync queueManager, 
            IClipboard clipboard) 
            : base(eventAggregator, decoder, queueManager, clipboard)
        {
            DisplayName = "Saga";
        }

        protected override bool IsMatchingHeader(HeaderInfo header)
        {
            return header.Key.EndsWith(".SagaType", StringComparison.OrdinalIgnoreCase)             ||
                   header.Key.EndsWith(".SagaDataType", StringComparison.OrdinalIgnoreCase)         ||
                   header.Key.EndsWith(".OriginatingSagaId", StringComparison.OrdinalIgnoreCase)    ||
                   header.Key.EndsWith(".OriginatingSagaType", StringComparison.OrdinalIgnoreCase)  ||
                   header.Key.EndsWith(".IsSagaTimeoutMessage", StringComparison.OrdinalIgnoreCase) ||
                   header.Key.EndsWith(".SagaId", StringComparison.OrdinalIgnoreCase);
        }
    }
}