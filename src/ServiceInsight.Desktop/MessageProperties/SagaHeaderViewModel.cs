using System;
using System.Collections.Generic;
using System.ComponentModel;
using Caliburn.PresentationFramework.ApplicationModel;
using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.Core.MessageDecoders;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.MessageProperties
{
    public class SagaHeaderViewModel : HeaderInfoViewModelBase, ISagaHeaderViewModel
    {
        public SagaHeaderViewModel(
            IEventAggregator eventAggregator, 
            IContentDecoder<IList<HeaderInfo>> decoder, 
            IQueueManagerAsync queueManager) 
            : base(eventAggregator, decoder, queueManager)
        {
            DisplayName = "Saga";
        }

        [Description("Saga type")]
        public string SagaType { get; private set; }

        [Description("Data type of the saga")]
        public string SagaDataType { get; private set; }

        [Description("Id of the originating saga message")]
        public string OriginatingSagaId { get; private set; }

        [Description("Type of the originating saga")]
        public string OriginatingSagaType { get; private set; }

        [Description("Is this a saga timeout message?")]
        public string IsSagaTimeoutMessage { get; private set; }

        [Description("Id of the sage")]
        public string SagaId { get; private set; }

        protected override void MapHeaderKeys()
        {
            ConditionsMap.Add(h => h.Key.EndsWith(".SagaType", StringComparison.OrdinalIgnoreCase), h => SagaType = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith(".SagaDataType", StringComparison.OrdinalIgnoreCase), h => SagaDataType = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith(".OriginatingSagaId", StringComparison.OrdinalIgnoreCase), h => OriginatingSagaId = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith(".OriginatingSagaType", StringComparison.OrdinalIgnoreCase), h => OriginatingSagaType = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith(".IsSagaTimeoutMessage", StringComparison.OrdinalIgnoreCase), h => IsSagaTimeoutMessage = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith(".SagaId", StringComparison.OrdinalIgnoreCase), h => SagaId = h.Value);
        }

        protected override void ClearHeaderValues()
        {
            SagaType = null;
            SagaDataType = null;
            OriginatingSagaId = null;
            OriginatingSagaType = null;
            IsSagaTimeoutMessage = null;
            SagaId = null;
        }
    }
}