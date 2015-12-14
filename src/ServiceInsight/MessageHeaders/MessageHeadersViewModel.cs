namespace Particular.ServiceInsight.Desktop.MessageHeaders
{
    using System.Linq;
    using Caliburn.Micro;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using Particular.ServiceInsight.Desktop.MessageList;
    using ReactiveUI;

    public class MessageHeadersViewModel : Screen, IHandle<SelectedMessageChanged>
    {
        readonly MessageSelectionContext selection;

        public MessageHeadersViewModel(MessageSelectionContext selectionContext)
        {
            selection = selectionContext;
            KeyValues = new ReactiveList<MessageHeaderKeyValue> { ResetChangeThreshold = 0 };
        }

        public ReactiveList<MessageHeaderKeyValue> KeyValues { get; }

        public void Handle(SelectedMessageChanged @event)
        {
            KeyValues.Clear();
            var storedMessage = selection.SelectedMessage;
            if (storedMessage == null) return;
            var headers = storedMessage.Headers;

            using (KeyValues.SuppressChangeNotifications())
            {
                KeyValues.AddRange(headers.Select(h => new MessageHeaderKeyValue
                {
                    Key = h.Key,
                    Value = h.Value
                }));
            }
        }
    }
}