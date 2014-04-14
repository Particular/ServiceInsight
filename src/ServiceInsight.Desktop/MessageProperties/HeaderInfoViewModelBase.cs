using System;
using System.Collections.Generic;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.Core.MessageDecoders;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Explorer;
using NServiceBus.Profiler.Desktop.Explorer.QueueExplorer;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.MessageProperties
{
    public abstract class HeaderInfoViewModelBase : Screen, IHeaderInfoViewModel
    {
        private readonly IContentDecoder<IList<HeaderInfo>> _decoder;
        
        protected HeaderInfoViewModelBase (
            IEventAggregator eventAggregator, 
            IContentDecoder<IList<HeaderInfo>> decoder, 
            IQueueManager queueManager)
        {
            _decoder = decoder;
            EventAggregator = eventAggregator;
            QueueManager = queueManager;
            ConditionsMap = new Dictionary<Func<HeaderInfo, bool>, Action<HeaderInfo>>();
            MapHeaderKeys();
        }

        protected IDictionary<Func<HeaderInfo, bool>, Action<HeaderInfo>> ConditionsMap { get; private set; }

        protected IQueueManager QueueManager { get; private set; }

        protected IEventAggregator EventAggregator { get; private set; }

        protected IList<HeaderInfo> Headers { get; private set; }

        protected MessageBody SelectedMessage { get; private set; }

        protected Queue SelectedQueue { get; private set; } 

        public void Handle(SelectedExplorerItemChanged @event)
        {
            var queue = @event.SelectedExplorerItem.As<QueueExplorerItem>();
            SelectedQueue = queue != null ? queue.Queue : null;
        }

        public void Handle(SelectedMessageChanged @event)
        {
            SelectedMessage = @event.Message;
            ClearHeaderValues();

            if (SelectedMessage == null)
            {
                Headers = null;
            }
            else
            {
                Headers = DecodeHeader(SelectedMessage);
                OnItemsLoaded();
            }
        }

        protected virtual IList<HeaderInfo> DecodeHeader(MessageBody message)
        {
            var headers = message.HeaderRaw;
            var decodedResult = _decoder.Decode(headers);
            
            return decodedResult.IsParsed ? decodedResult.Value : new HeaderInfo[0];
        }

        protected void OnItemsLoaded()
        {
            foreach (var condition in ConditionsMap)
            {
                foreach (var header in Headers)
                {
                    if (condition.Key(header))
                    {
                        condition.Value(header);
                    }
                }
            }

            OnMessagePropertiesLoaded();
        }

        protected virtual void OnMessagePropertiesLoaded()
        {
        }

        protected abstract void MapHeaderKeys();

        protected abstract void ClearHeaderValues();
    }
}