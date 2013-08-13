using Caliburn.PresentationFramework.Screens;
using Particular.ServiceInsight.Desktop.Core.MessageDecoders;
using Particular.ServiceInsight.Desktop.Events;
using Particular.ServiceInsight.Desktop.Models;

namespace Particular.ServiceInsight.Desktop.MessageViewers.JsonViewer
{
    public class JsonMessageViewModel : Screen, IJsonMessageViewModel
    {
        private IJsonMessageView _messageView;
        private readonly IContentDecoder<string> _messageDecoder;

        public JsonMessageViewModel(IContentDecoder<string> messageDecoder)
        {
            _messageDecoder = messageDecoder;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            DisplayName = "Json";
        }

        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            _messageView = (IJsonMessageView)view;
            OnSelectedMessageChanged();
        }

        public MessageBody SelectedMessage { get; set; }

        public void OnSelectedMessageChanged()
        {
            if (_messageView == null) return;

            _messageView.Clear();

            if (SelectedMessage != null)
            {
                if (SelectedMessage.Body != null)
                {
                    _messageView.Display(SelectedMessage.Body);
                }
                else
                {
                    var json = _messageDecoder.Decode(SelectedMessage.BodyRaw);
                    if (json.IsParsed)
                    {
                        _messageView.Display(json.Value);
                    }
                }
            }
        }

        public void Handle(SelectedMessageChanged @event)
        {
            if (@event.SelectedMessage == null)
            {
                SelectedMessage = null;
            }
        }

        public void Handle(MessageBodyLoaded @event)
        {
            SelectedMessage = @event.Message;
        }
    }
}