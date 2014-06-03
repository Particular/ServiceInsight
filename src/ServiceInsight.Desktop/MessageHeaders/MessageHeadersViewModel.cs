namespace Particular.ServiceInsight.Desktop.MessageHeaders
{
    using System.Linq;
    using Caliburn.Micro;
    using Core.UI;
    using Events;
    using ReactiveUI;
    using Shell.Menu;

    public class MessageHeadersViewModel : Screen, IHaveContextMenu, IHandle<SelectedMessageChanged>
    {
        IMessageHeadersView view;

        public MessageHeadersViewModel()
        {
            KeyValues = new ReactiveList<MessageHeaderKeyValue> { ResetChangeThreshold = 0 };

            ContextMenuItems = new BindableCollection<IMenuItem>
            {
                new MenuItem("Copy To Clipboard", new RelayCommand(CopyHeadersToClipboard))
            };
        }

        public IObservableCollection<IMenuItem> ContextMenuItems { get; private set; }

        public void OnContextMenuOpening()
        {
        }

        public ReactiveList<MessageHeaderKeyValue> KeyValues { get; private set; }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            this.view = (IMessageHeadersView)view;
        }

        public void Handle(SelectedMessageChanged @event)
        {
            KeyValues.Clear();
            var storedMessage = @event.Message;
            if (storedMessage == null) return;
            var headers = storedMessage.Headers;

            KeyValues.AddRange(headers.Select(h => new MessageHeaderKeyValue
            {
                Key = h.Key,
                Value = h.Value
            }));
        }

        void CopyHeadersToClipboard()
        {
            view.CopyRowsToClipboard();
        }
    }
}