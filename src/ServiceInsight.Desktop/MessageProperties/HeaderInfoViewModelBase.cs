namespace Particular.ServiceInsight.Desktop.MessageProperties
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;
    using Framework.Rx;
    using Models;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using Particular.ServiceInsight.Desktop.Framework.MessageDecoders;

    public abstract class HeaderInfoViewModelBase : RxScreen, IHeaderInfoViewModel
    {
        IContentDecoder<IList<HeaderInfo>> decoder;

        protected HeaderInfoViewModelBase(
            IEventAggregator eventAggregator,
            IContentDecoder<IList<HeaderInfo>> decoder)
        {
            this.decoder = decoder;
            EventAggregator = eventAggregator;
            ConditionsMap = new Dictionary<Func<HeaderInfo, bool>, Action<HeaderInfo>>();
            MapHeaderKeys();
        }

        protected IDictionary<Func<HeaderInfo, bool>, Action<HeaderInfo>> ConditionsMap { get; private set; }

        protected IEventAggregator EventAggregator { get; private set; }

        protected IList<HeaderInfo> Headers { get; private set; }

        protected MessageBody SelectedMessage { get; private set; }

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