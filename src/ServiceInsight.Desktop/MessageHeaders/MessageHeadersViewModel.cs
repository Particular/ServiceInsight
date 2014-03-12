namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    using System.Linq;
    using Caliburn.PresentationFramework;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Screens;
    using Events;

    public interface IMessageHeadersViewModel : IScreen, IHandle<SelectedMessageChanged>
    {
        IObservableCollection<MessageHeaderKeyValue> KeyValues { get; }
    }

    public class MessageHeadersViewModel : Screen, IMessageHeadersViewModel
    {
        private IMessageHeadersView _view;
        private bool _autoFitted;

        public MessageHeadersViewModel()
        {
            KeyValues = new BindableCollection<MessageHeaderKeyValue>();
        }

        public IObservableCollection<MessageHeaderKeyValue> KeyValues { get; private set; }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _view = (IMessageHeadersView) view;
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

            AutoFitKeys();
        }

        private void AutoFitKeys()
        {
            if(_autoFitted) return;

            _view.AutoFit();
            _autoFitted = true;
        }
    }
}