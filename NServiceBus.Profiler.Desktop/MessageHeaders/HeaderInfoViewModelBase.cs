using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core;
using NServiceBus.Profiler.Core.MessageDecoders;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Explorer;
using NServiceBus.Profiler.Desktop.Explorer.QueueExplorer;

namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    public abstract class HeaderInfoViewModelBase : Screen, IHeaderInfoViewModel
    {
        private readonly IContentDecoder<IList<HeaderInfo>> _decoder;

        protected HeaderInfoViewModelBase (
            IEventAggregator eventAggregator, 
            IContentDecoder<IList<HeaderInfo>> decoder, 
            IQueueManagerAsync queueManager)
        {
            _decoder = decoder;
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

        public void Handle(SelectedExplorerItemChanged @event)
        {
            var queue = @event.SelectedExplorerItem.As<QueueExplorerItem>();
            if (queue != null)
            {
                SelectedQueue = queue.Queue;
            }
        }

        public virtual void Handle(MessageBodyLoaded @event)
        {
            Items.Clear();

            SelectedMessage = @event.Message;

            if (SelectedMessage != null)
            {
                var headers = DecodeHeader(SelectedMessage);
                OnItemsLoaded(headers);
            }
        }

        protected IEnumerable<HeaderInfo> DecodeHeader(MessageBody message)
        {
            var headers = message.Headers;
            var decodeResult = _decoder.Decode(headers);
            
            return decodeResult.IsParsed ? decodeResult.Value : Enumerable.Empty<HeaderInfo>();
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

        public virtual void Handle(SelectedMessageChanged @event)
        {
            if (@event.SelectedMessage == null)
            {
                Items.Clear();
                SelectedMessage = null;
            }
        }
    }
}