namespace ServiceInsight.MessageHeaders
{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Framework.Events;
    using MessageList;

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

            foreach (var item in headers)
            {
                KeyValues.Add(new MessageHeaderKeyValue { Key = item.Key, Value = item.Value });
            }
        }
    }
}