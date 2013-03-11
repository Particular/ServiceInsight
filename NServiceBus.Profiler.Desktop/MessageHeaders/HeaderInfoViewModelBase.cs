using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Xml.Serialization;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using ExceptionHandler;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core;
using NServiceBus.Profiler.Core.MessageDecoders;
using DevExpress.Xpf.Core;

namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    public abstract class HeaderInfoViewModelBase : Screen, IHeaderInfoViewModel
    {
        private readonly IClipboard _clipboard;
        private readonly IContentDecoder<IList<HeaderInfo>> _decoder;

        protected HeaderInfoViewModelBase (
            IEventAggregator eventAggregator, 
            IContentDecoder<IList<HeaderInfo>> decoder, 
            IQueueManagerAsync queueManager,
            IClipboard clipboard)
        {
            _decoder = decoder;
            _clipboard = clipboard;
            EventAggregator = eventAggregator;
            QueueManager = queueManager;
            Items = new BindableCollection<HeaderInfo>();
        }

        public IObservableCollection<HeaderInfo> Items { get; private set; }

        public abstract ImageSource GroupImage { get; }

        public virtual TabPriority Order
        {
            get { return TabPriority.Regular; }
        }

        protected IQueueManagerAsync QueueManager { get; private set; }

        protected IEventAggregator EventAggregator { get; private set; }

        protected MessageBody SelectedMessage { get; private set; }

        protected Queue SelectedQueue { get; private set; } 

        public void Handle(SelectedQueueChangedEvent @event)
        {
            SelectedQueue = @event.SelectedQueue;
        }

        public virtual void Handle(MessageBodyLoadedEvent @event)
        {
            Items.Clear();

            SelectedMessage = @event.Message;

            if (SelectedMessage != null)
            {
                var headers = SelectedMessage.Headers;
                var decodeResult = _decoder.Decode(headers);

                if (decodeResult.IsParsed)
                {
                    OnItemsLoaded(decodeResult.Value);
                }
            }
        }

        protected void OnItemsLoaded(IEnumerable<HeaderInfo> headers)
        {
            foreach (var header in headers)
            {
                if (IsMatchingHeader(header))
                {
                    Items.Add(header);
                }
            }
        }

        protected abstract bool IsMatchingHeader(HeaderInfo header);

        public virtual bool CanCopyHeaderInfo()
        {
            return Items != null;
        }

        public virtual void CopyHeaderInfo()
        {
            var serializer = new XmlSerializer(typeof (HeaderInfo[]));
            using (var stream = new MemoryStream())
            {
                var headers = new List<HeaderInfo>(Items);
                serializer.Serialize(stream, headers.ToArray());
                var content = stream.ReadString();
                _clipboard.CopyTo(content);
            }
        }

        public virtual void Handle(SelectedMessageChangedEvent @event)
        {
            if (@event.SelectedMessage == null)
            {
                Items.Clear();
                SelectedMessage = null;
            }
        }
    }
}