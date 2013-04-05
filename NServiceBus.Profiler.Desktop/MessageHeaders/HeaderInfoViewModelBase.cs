using System;
using System.Collections.Generic;
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
            if (queue != null)
            {
                SelectedQueue = queue.Queue;
            }
        }

        public virtual void Handle(MessageBodyLoaded @event)
        {
            ClearHeaderValues();
            SelectedMessage = @event.Message;

            if (SelectedMessage != null)
            {
                Headers = DecodeHeader(SelectedMessage);
                OnItemsLoaded();
            }
        }

        protected IList<HeaderInfo> DecodeHeader(MessageBody message)
        {
            var headers = message.Headers;
            var decodeResult = _decoder.Decode(headers);
            
            return decodeResult.IsParsed ? decodeResult.Value : new HeaderInfo[0];
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
        }

        protected abstract void MapHeaderKeys();

        protected abstract void ClearHeaderValues();

        protected string GetConditionalValue(Func<HeaderInfo, bool> condition, HeaderInfo header)
        {
            return condition(header) ? header.Value : null;
        }

        public virtual void Handle(SelectedMessageChanged @event)
        {
            if (@event.SelectedMessage == null)
            {
                SelectedMessage = null;
                Headers = null;
                ClearHeaderValues();
            }
        }
    }
}