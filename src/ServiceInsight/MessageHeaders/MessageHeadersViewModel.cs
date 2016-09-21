namespace ServiceInsight.MessageHeaders
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using DynamicData;
    using Framework;
    using Framework.Rx;
    using Pirac;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.MessageList;

    public class MessageHeadersViewModel : RxScreen
    {
        readonly MessageSelectionContext selection;

        public MessageHeadersViewModel(MessageSelectionContext selectionContext, IRxEventAggregator eventAggregator)
        {
            selection = selectionContext;
            KeyValues = new ObservableCollection<MessageHeaderKeyValue>();
            eventAggregator.GetEvent<SelectedMessageChanged>().ObserveOnPiracMain().Subscribe(Handle);
        }

        public ObservableCollection<MessageHeaderKeyValue> KeyValues { get; }

        void Handle(SelectedMessageChanged @event)
        {
            KeyValues.Clear();
            var storedMessage = selection.SelectedMessage;
            if (storedMessage == null)
            {
                return;
            }

            var headers = storedMessage.Headers;

            KeyValues.AddRange(headers.Select(h => new MessageHeaderKeyValue
            {
                Key = h.Key,
                Value = h.Value
            }));
        }
    }
}