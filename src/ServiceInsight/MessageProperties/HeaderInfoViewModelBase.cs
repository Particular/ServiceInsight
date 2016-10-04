namespace ServiceInsight.MessageProperties
{
    using System;
    using System.Collections.Generic;
    using Framework;
    using Framework.Rx;
    using Models;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.MessageDecoders;
    using ServiceInsight.MessageList;

    public abstract class HeaderInfoViewModelBase : RxScreen
    {
        IContentDecoder<IList<HeaderInfo>> decoder;

        protected HeaderInfoViewModelBase(
            IRxEventAggregator eventAggregator,
            IContentDecoder<IList<HeaderInfo>> decoder,
            MessageSelectionContext selectionContext)
        {
            this.decoder = decoder;
            EventAggregator = eventAggregator;
            Selection = selectionContext;
            ConditionsMap = new Dictionary<Func<HeaderInfo, bool>, Action<HeaderInfo>>();
            MapHeaderKeys();
            eventAggregator.GetEvent<SelectedMessageChanged>().Subscribe(Handle);
        }

        protected IDictionary<Func<HeaderInfo, bool>, Action<HeaderInfo>> ConditionsMap { get; }

        protected IRxEventAggregator EventAggregator { get; }

        protected IList<HeaderInfo> Headers { get; private set; }

        protected MessageSelectionContext Selection { get; }

        void Handle(SelectedMessageChanged @event)
        {
            ClearHeaderValues();

            if (Selection.SelectedMessage == null)
            {
                Headers = null;
            }
            else
            {
                Headers = DecodeHeader(Selection.SelectedMessage);
                OnItemsLoaded();
            }
        }

        protected virtual IList<HeaderInfo> DecodeHeader(MessageBody message)
        {
            var headers = message.HeaderRaw;
            var decodedResult = decoder.Decode(headers);

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