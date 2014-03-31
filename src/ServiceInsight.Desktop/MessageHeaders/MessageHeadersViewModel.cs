namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    using System.Linq;
    using Caliburn.PresentationFramework;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Screens;
    using Core.UI;
    using Events;
    using Shell.Menu;

    public interface IMessageHeadersViewModel : IScreen, IHaveContextMenu, IHandle<SelectedMessageChanged>
    {
        IObservableCollection<MessageHeaderKeyValue> KeyValues { get; }
    }

    public class MessageHeadersViewModel : Screen, IMessageHeadersViewModel
    {
        private readonly IMenuItem _copyAllHeaders;
        private IMessageHeadersView _view;
        private bool _autoFitted;

        public MessageHeadersViewModel()
        {
            KeyValues = new BindableCollection<MessageHeaderKeyValue>();
            _copyAllHeaders = new MenuItem("Copy To Clipboard", new RelayCommand(CopyHeadersToClipboard));

            ContextMenuItems = new BindableCollection<IMenuItem>
            {
                _copyAllHeaders
            };
        }

        public IObservableCollection<IMenuItem> ContextMenuItems { get; private set; }
        
        public void OnContextMenuOpening()
        {
        }

        public IObservableCollection<MessageHeaderKeyValue> KeyValues { get; private set; }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _view = (IMessageHeadersView) view;
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

            AutoFitKeys();
        }

        private void AutoFitKeys()
        {
            if(_autoFitted) return;

            _view.AutoFit();
            _autoFitted = true;
        }

        private void CopyHeadersToClipboard()
        {
            _view.CopyRowsToClipboard();
        }
    }
}