using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using ExceptionHandler;
using NServiceBus.Profiler.Bus.Properties;
using NServiceBus.Profiler.Common;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Common.Plugins;
using NServiceBus.Profiler.Core;
using NServiceBus.Profiler.Core.MessageDecoders;
using DevExpress.Xpf.Core;

namespace NServiceBus.Profiler.Bus
{
    public class HeaderInfoViewModel : Screen, IHeaderInfoViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IMessageDecoder<string> _decoder;
        private readonly IQueueManagerAsync _queueManager;
        private readonly IClipboard _clipboard;
        private MessageBody _selectedMessage;
        private Queue _selectedQueue;

        public HeaderInfoViewModel(
            IEventAggregator eventAggregator, 
            IMessageDecoder<string> decoder, 
            IQueueManagerAsync queueManager,
            IClipboard clipboard)
        {
            _eventAggregator = eventAggregator;
            _decoder = decoder;
            _queueManager = queueManager;
            _clipboard = clipboard;
            Items = new BindableCollection<HeaderInfo>();
            ContextMenuItems = new List<PluginContextMenu>
            {
                new PluginContextMenu("ReturnToSourceQueue", new RelayCommand(ReturnToSource, CanReturnToSource), 90)
                {
                    DisplayName = "Return To Source Queue",
                    Image = Resources.MessageReturn
                },
                new PluginContextMenu("CopyHeaderInfo", new RelayCommand(CopyHeaderInfo, CanCopyHeaderInfo))
                {
                    DisplayName = "Copy Header Info",
                }
            };
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            DisplayName = "NServiceBus";
        }

        public virtual IObservableCollection<HeaderInfo> Items { get; private set; }

        public MessageBody SelectedMessage { get { return _selectedMessage; } }

        public void Handle(SelectedQueueChangedEvent @event)
        {
            _selectedQueue = @event.SelectedQueue;
        }

        public virtual void Handle(MessageBodyLoadedEvent @event)
        {
            Items.Clear();

            _selectedMessage = @event.Message;

            var headers = _selectedMessage.Headers;
            var decoded = _decoder.Decode(headers);
            var items = Deserialize(decoded);

            Items.AddRange(items);
        }

        public virtual bool CanReturnToSource()
        {
            return _selectedQueue != null &&
                   !_selectedQueue.IsRemoteQueue() &&
                   _selectedMessage != null &&
                   Items.Count > 0 &&
                   Items.Any(header => header.Key.Equals(HeaderInfo.FailedQueueKey, StringComparison.InvariantCultureIgnoreCase));
        }

        public virtual void ReturnToSource()
        {
            if (!CanReturnToSource()) return;

            var destinationQueueName = Items.First(header => header.Key == HeaderInfo.FailedQueueKey);
            var destinationAddress = Address.Parse(destinationQueueName.Value);
            var queues = _queueManager.GetQueues();
            var destinationQueue = queues.FirstOrDefault(q => q.Address == destinationAddress);

            if (destinationQueue != null)
            {
                _queueManager.MoveMessage(_selectedQueue, destinationQueue, _selectedMessage.Id);
                _eventAggregator.Publish(new MessageRemovedFromQueueEvent { Message = _selectedMessage });
            }
        }

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

        private static IEnumerable<HeaderInfo> Deserialize(string content)
        {
            try
            {
                var serializer = new XmlSerializer(typeof (HeaderInfo[]));
                var deserialized = (HeaderInfo[])serializer.Deserialize(new StringReader(content));
                return deserialized;
            }
            catch (InvalidOperationException)
            {
                return Enumerable.Empty<HeaderInfo>();
            }
        }

        public IList<PluginContextMenu> ContextMenuItems
        {
            get; private set;
        }

        public int TabOrder
        {
            get { return 0; }
        }

        public void Handle(SelectedMessageChangedEvent @event)
        {
            if (@event.SelectedMessage == null)
            {
                Items.Clear();
                _selectedMessage = null;
            }
        }
    }
}