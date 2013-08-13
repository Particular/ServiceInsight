﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Caliburn.PresentationFramework.ApplicationModel;
using DevExpress.Xpf.Core;
using ExceptionHandler;
using Particular.ServiceInsight.Desktop.Core;
using Particular.ServiceInsight.Desktop.Core.MessageDecoders;
using Particular.ServiceInsight.Desktop.ExtensionMethods;
using Particular.ServiceInsight.Desktop.MessageHeaders.Editors;
using Particular.ServiceInsight.Desktop.Models;

namespace Particular.ServiceInsight.Desktop.MessageHeaders
{
    [TypeConverter(typeof(HeaderInfoTypeConverter))]
    public class GeneralHeaderViewModel : HeaderInfoViewModelBase, IGeneralHeaderViewModel
    {
        private readonly IClipboard _clipboard;
        private readonly IContentDecoder<IList<HeaderInfo>> _decoder;

        public GeneralHeaderViewModel(
            IEventAggregator eventAggregator, 
            IContentDecoder<IList<HeaderInfo>> decoder, 
            IQueueManagerAsync queueManager, 
            IClipboard clipboard) 
            : base(eventAggregator, decoder, queueManager)
        {
            _clipboard = clipboard;
            _decoder = decoder;
            DisplayName = "General";
        }

        public virtual bool CanCopyHeaderInfo()
        {
            return !Headers.IsEmpty();
        }

        public virtual void CopyHeaderInfo()
        {
            var serializer = new XmlSerializer(typeof(HeaderInfo[]));
            using (var stream = new MemoryStream())
            {
                var headers = new List<HeaderInfo>(Headers);
                serializer.Serialize(stream, headers.ToArray());
                var content = stream.ReadString();
                _clipboard.CopyTo(content);
            }
        }

        [Description("NServiceBus version")]
        public string Version { get; set; }

        [Description("Type of the message")]
        public string EnclosedMessageTypes { get; set; }

        [Description("Number of retries")]
        public string Retries { get; set; }

        [Description("Id of the message this relates to")]
        public string RelatedTo { get; set; }

        [Description("Content type of the message")]
        public string ContentType { get; set; }

        [Description("Is this message deferred?")]
        public string IsDeferedMessage { get; set; }

        [Description("Conversation Identifier")]
        public string ConversationId { get; set; }

        [Editor(typeof(ResizableDropDownEditor), typeof(ResizableDropDownEditor))]
        [Description("Raw header information")]
        public string HeaderContent { get; set; }

        protected override IList<HeaderInfo> DecodeHeader(MessageBody message)
        {
            HeaderContent = Encoding.UTF8.GetString(message.Headers);

            var decodedResult = _decoder.Decode(message.Headers);
            return decodedResult.IsParsed ? decodedResult.Value : new HeaderInfo[0];
        }

        protected override void MapHeaderKeys()
        {
            ConditionsMap.Add(h => h.Key.EndsWith("Version", StringComparison.OrdinalIgnoreCase), h => Version = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith("EnclosedMessageTypes", StringComparison.OrdinalIgnoreCase), h => EnclosedMessageTypes = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith("Retries", StringComparison.OrdinalIgnoreCase), h => Retries = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith("RelatedTo", StringComparison.OrdinalIgnoreCase), h => RelatedTo = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith("ContentType", StringComparison.OrdinalIgnoreCase), h => ContentType = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith("IsDeferedMessage", StringComparison.OrdinalIgnoreCase), h => IsDeferedMessage = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith("ConversationId", StringComparison.OrdinalIgnoreCase), h => ConversationId = h.Value);
        }

        protected override void ClearHeaderValues()
        {
            Version = null;
            EnclosedMessageTypes = null;
            Retries = null;
            RelatedTo = null;
            ContentType = null;
            IsDeferedMessage = null;
            ConversationId = null;
            HeaderContent = null;
        }
    }
}