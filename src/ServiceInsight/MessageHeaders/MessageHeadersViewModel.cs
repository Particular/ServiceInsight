namespace Particular.ServiceInsight.Desktop.MessageHeaders
{
    using System.Linq;
    using Caliburn.Micro;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using ReactiveUI;

    public class MessageHeadersViewModel : Screen, IHandle<SelectedMessageChanged>
    {
        public MessageHeadersViewModel()
        {
            KeyValues = new ReactiveList<MessageHeaderKeyValue> { ResetChangeThreshold = 0 };
        }

        public ReactiveList<MessageHeaderKeyValue> KeyValues { get; private set; }

        public void Handle(SelectedMessageChanged @event)
        {
            KeyValues.Clear();
            var storedMessage = @event.Message;
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