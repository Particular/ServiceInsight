namespace ServiceInsight.MessageHeaders
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Caliburn.Micro;
    using DynamicData;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.MessageList;

    public class MessageHeadersViewModel : Screen, IHandle<SelectedMessageChanged>
    {
        readonly MessageSelectionContext selection;

        public MessageHeadersViewModel(MessageSelectionContext selectionContext)
        {
            selection = selectionContext;
            KeyValues = new ObservableCollection<MessageHeaderKeyValue>();
        }

        public ObservableCollection<MessageHeaderKeyValue> KeyValues { get; }

        public void Handle(SelectedMessageChanged @event)
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