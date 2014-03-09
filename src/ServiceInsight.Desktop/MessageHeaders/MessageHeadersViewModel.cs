namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    using System;
    using System.Linq;
    using Caliburn.PresentationFramework;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Screens;
    using Events;

    public interface IMessageHeadersViewModel : IScreen, IHandle<SelectedMessageChanged>
    {
    }

    public class MessageHeadersViewModel : Screen, IMessageHeadersViewModel
    {
        public IObservableCollection<MessageHeaderKeyValue> KeyValues { get; set; }

        public MessageHeadersViewModel()
        {
            KeyValues = new BindableCollection<MessageHeaderKeyValue>();
        }

        public void Handle(SelectedMessageChanged @event)
        {
            var storedMessage = @event.Message;
            var headers = storedMessage.Headers;

            KeyValues.Clear();
            KeyValues.AddRange(headers.Select(h => new MessageHeaderKeyValue
            {
                Key = h.Key,
                Value = h.Value
            }));
        }
    }

    public class MessageHeaderKeyValue : PropertyChangedBase
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}