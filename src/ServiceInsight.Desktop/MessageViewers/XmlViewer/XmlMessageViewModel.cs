using System.Text;
using System.Xml;
using Caliburn.PresentationFramework.Screens;
using ExceptionHandler;
using NServiceBus.Profiler.Desktop.Core.MessageDecoders;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.ExtensionMethods;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.MessageViewers.XmlViewer
{
    public class XmlMessageViewModel : Screen, IXmlMessageViewModel
    {
        private readonly IContentDecoder<XmlDocument> _xmlDecoder;
        private readonly IClipboard _clipboard;
        private IXmlMessageView _messageView;

        public XmlMessageViewModel(
            IContentDecoder<XmlDocument> xmlDecoder,
            IClipboard clipboard)
        {
            _xmlDecoder = xmlDecoder;
            _clipboard = clipboard;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            DisplayName = "Xml";
        }

        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            _messageView = (IXmlMessageView)view;
            OnSelectedMessageChanged();
        }

        public MessageBody SelectedMessage { get; set; }

        public void OnSelectedMessageChanged()
        {
            if(_messageView == null) return;

            _messageView.Clear();
            ShowMessageBody();
        }

        public bool CanCopyMessageXml()
        {
            return SelectedMessage != null;
        }

        public void CopyMessageXml()
        {
            var content = GetMessageBody();
            if (!content.IsEmpty())
            {
                _clipboard.CopyTo(content);
            }
        }

        public void Handle(SelectedMessageChanged @event)
        {
            SelectedMessage = @event.Message;
        }

        private void ShowMessageBody()
        {
            if (SelectedMessage == null) return;
            _messageView.Display(GetMessageBody());
        }

        private string GetMessageBody()
        {
            if (SelectedMessage == null || SelectedMessage.Body == null) return string.Empty;

            var bytes = Encoding.Default.GetBytes(SelectedMessage.Body);
            var xml = _xmlDecoder.Decode(bytes);
            return xml.IsParsed ? xml.Value.GetFormatted() : string.Empty;
        }
    }
}