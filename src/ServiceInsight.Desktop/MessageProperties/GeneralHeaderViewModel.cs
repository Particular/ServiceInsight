using System;
using System.Collections.Generic;
using System.ComponentModel;
using Caliburn.PresentationFramework.ApplicationModel;
using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.Core.MessageDecoders;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.MessageProperties
{
    public class GeneralHeaderViewModel : HeaderInfoViewModelBase, IGeneralHeaderViewModel
    {

        private readonly IContentDecoder<IList<HeaderInfo>> _decoder;

        public GeneralHeaderViewModel(
            IEventAggregator eventAggregator, 
            IContentDecoder<IList<HeaderInfo>> decoder, 
            IQueueManagerAsync queueManager)
            : base(eventAggregator, decoder, queueManager)
        {
            _decoder = decoder;
            DisplayName = "General";
        }

        [Description("NServiceBus version")]
        public string Version { get; private set; }

        [Description("Type of the message")]
        public string EnclosedMessageTypes { get; private set; }

        [Description("Id of the message this relates to")]
        public string RelatedTo { get; private set; }

        [Description("Content type of the message")]
        public string ContentType { get; private set; }

        [Description("Is this message deferred?")]
        public string IsDeferedMessage { get; private set; }

        [Description("Conversation Identifier")]
        public string ConversationId { get; private set; }

        [Description("Message Identifier")]
        public string MessageId { get; private set; }

        [Description("Raw header information")]
        public string HeaderContent { get; private set; }

        protected override IList<HeaderInfo> DecodeHeader(MessageBody message)
        {
            var headerDecoder = new MessageHeaderDecoder(_decoder, message);
            HeaderContent = headerDecoder.RawHeader;

            return headerDecoder.DecodedHeaders;
        }

        protected override void MapHeaderKeys()
        {
            ConditionsMap.Add(h => h.Key.EndsWith(MessageHeaderKeys.Version, StringComparison.OrdinalIgnoreCase), h => Version = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith(MessageHeaderKeys.EnclosedMessageTypes, StringComparison.OrdinalIgnoreCase), h => EnclosedMessageTypes = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith(MessageHeaderKeys.RelatedTo, StringComparison.OrdinalIgnoreCase), h => RelatedTo = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith(MessageHeaderKeys.MessageId, StringComparison.OrdinalIgnoreCase), h => MessageId = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith(MessageHeaderKeys.ContentType, StringComparison.OrdinalIgnoreCase), h => ContentType = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith(MessageHeaderKeys.IsDeferedMessage, StringComparison.OrdinalIgnoreCase), h => IsDeferedMessage = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith(MessageHeaderKeys.ConversationId, StringComparison.OrdinalIgnoreCase), h => ConversationId = h.Value);
        }

        protected override void ClearHeaderValues()
        {
            Version = null;
            EnclosedMessageTypes = null;
            RelatedTo = null;
            ContentType = null;
            IsDeferedMessage = null;
            ConversationId = null;
            MessageId = null;
            HeaderContent = null;
        }

    }
}