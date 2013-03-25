using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Xml.Serialization;
using Caliburn.PresentationFramework.ApplicationModel;
using DevExpress.Xpf.Core;
using ExceptionHandler;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core;
using NServiceBus.Profiler.Core.MessageDecoders;
using NServiceBus.Profiler.Desktop.Properties;
using NServiceBus.Profiler.Common.ExtensionMethods;

namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    public class GeneralHeaderViewModel : HeaderInfoViewModelBase, IGeneralHeaderDisplay
    {
        private readonly IClipboard _clipboard;

        public GeneralHeaderViewModel(
            IEventAggregator eventAggregator, 
            IContentDecoder<IList<HeaderInfo>> decoder, 
            IQueueManagerAsync queueManager, 
            IClipboard clipboard) 
            : base(eventAggregator, decoder, queueManager)
        {
            _clipboard = clipboard;
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

        public virtual bool CanCopyHeaderInfo()
        {
            return Items != null && Items.Any();
        }

        public virtual void CopyHeaderInfo()
        {
            var serializer = new XmlSerializer(typeof(HeaderInfo[]));
            using (var stream = new MemoryStream())
            {
                var headers = new List<HeaderInfo>(Items);
                serializer.Serialize(stream, headers.ToArray());
                var content = stream.ReadString();
                _clipboard.CopyTo(content);
            }
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