namespace ServiceInsight.MessageProperties
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using ExtensionMethods;
    using Framework;
    using Models;
    using ServiceInsight.Framework.MessageDecoders;
    using ServiceInsight.MessageList;

    public class ErrorHeaderViewModel : HeaderInfoViewModelBase, IPropertyDataProvider
    {
        public ErrorHeaderViewModel(
            IRxEventAggregator eventAggregator,
            IContentDecoder<IList<HeaderInfo>> decoder,
            MessageSelectionContext selectionContext)
            : base(eventAggregator, decoder, selectionContext)
        {
            DisplayName = "Errors";
        }

        [Description("Stack trace for the error")]
        public string ExceptionInfo { get; private set; }

        [Description("Failed Queue")]
        public string FailedQueue { get; private set; }

        [Description("The first time the message has failed")]
        public string TimeOfFailure { get; private set; }

        protected override void MapHeaderKeys()
        {
            ConditionsMap.Add(h => h.Key.EndsWith(MessageHeaderKeys.ExceptionStackTrace, StringComparison.OrdinalIgnoreCase), h => ExceptionInfo = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith(MessageHeaderKeys.FailedQueue, StringComparison.OrdinalIgnoreCase), h => FailedQueue = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith(MessageHeaderKeys.TimeOfFailure, StringComparison.OrdinalIgnoreCase), h => TimeOfFailure = h.Value.ParseHeaderDate().ToString());
        }

        protected override void ClearHeaderValues()
        {
            FailedQueue = null;
            ExceptionInfo = null;
            TimeOfFailure = null;
        }
    }
}