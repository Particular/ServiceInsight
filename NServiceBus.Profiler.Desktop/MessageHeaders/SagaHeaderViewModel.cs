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
    public class SagaHeaderViewModel : HeaderInfoViewModelBase
    {
        public SagaHeaderViewModel(
            IEventAggregator eventAggregator, 
            IContentDecoder<IList<HeaderInfo>> decoder, 
            IQueueManagerAsync queueManager) 
            : base(eventAggregator, decoder, queueManager)
        {
            DisplayName = "Saga";
        }

        public override ImageSource GroupImage
        {
            get { return Resources.HeaderSaga.ToBitmapImage(); }
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