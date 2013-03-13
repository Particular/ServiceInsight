using System.Xml;
using Caliburn.PresentationFramework.Screens;
using ExceptionHandler;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.ExtensionMethods;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core.MessageDecoders;

namespace NServiceBus.Profiler.Desktop.MessageViewers.XmlViewer
{
    public class XmlMessageViewModel : Screen, IXmlMessageViewModel
    {
        private readonly IContentDecoder<XmlDocument> _xmlDecoder;
        private readonly IContentDecoder<string> _stringDecoder;
        private readonly IClipboard _clipboard;
        private IXmlMessageView _messageView;

        public XmlMessageViewModel(
            IContentDecoder<XmlDocument> xmlDecoder,
            IContentDecoder<string> stringDecoder,
            IClipboard clipboard)
        {
            _xmlDecoder = xmlDecoder;
            _stringDecoder = stringDecoder;
            _clipboard = clipboard;

            //TODO: Add back context menu
//            ContextMenuItems = new List<PluginContextMenu>
//            {
//                new PluginContextMenu("CopyMessageXml", new RelayCommand(CopyMessageXml, CanCopyMessageXml))
//                {
//                    DisplayName = "Copy Message",
//                }
//            };
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

            if (SelectedMessage != null)
            {
                var xml = _xmlDecoder.Decode(SelectedMessage.BodyRaw);
                if (xml.IsParsed)
                {
                    _messageView.Display(xml.Value.GetFormatted());
                }
            }
        }

        public virtual bool CanCopyMessageXml()
        {
            return SelectedMessage != null;
        }

        public virtual void CopyMessageXml()
        {
            var content = _stringDecoder.Decode(SelectedMessage.BodyRaw);
            if (content.IsParsed)
            {
                _clipboard.CopyTo(content.Value);
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