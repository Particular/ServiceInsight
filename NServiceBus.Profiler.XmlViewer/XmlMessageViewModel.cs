using System.Collections.Generic;
using System.Xml;
using Caliburn.PresentationFramework.Screens;
using ExceptionHandler;
using NServiceBus.Profiler.Common;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.ExtensionMethods;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Common.Plugins;
using NServiceBus.Profiler.Core.MessageDecoders;

namespace NServiceBus.Profiler.XmlViewer
{
    public class XmlMessageViewModel : Screen, IXmlMessageViewModel
    {
        private readonly IMessageDecoder<XmlDocument> _xmlDecoder;
        private readonly IMessageDecoder<string> _stringDecoder;
        private readonly IClipboard _clipboard;
        private IXmlMessageView _messageView;

        public XmlMessageViewModel(
            IMessageDecoder<XmlDocument> xmlDecoder,
            IMessageDecoder<string> stringDecoder,
            IClipboard clipboard)
        {
            _xmlDecoder = xmlDecoder;
            _stringDecoder = stringDecoder;
            _clipboard = clipboard;
            ContextMenuItems = new List<PluginContextMenu>
            {
                new PluginContextMenu("CopyMessageXml", new RelayCommand(CopyMessageXml, CanCopyMessageXml))
                {
                    DisplayName = "Copy Message",
                }
            };
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
                var xml = _xmlDecoder.Decode(SelectedMessage.Content);
                _messageView.Display(xml.GetFormatted());
            }
        }

        public virtual bool CanCopyMessageXml()
        {
            return SelectedMessage != null;
        }

        public virtual void CopyMessageXml()
        {
            var content = _stringDecoder.Decode(SelectedMessage.Content);
            _clipboard.CopyTo(content);
        }

        public IList<PluginContextMenu> ContextMenuItems
        {
            get; private set;
        }

        public int TabOrder
        {
            get { return 10; }
        }

        public void Handle(SelectedMessageChangedEvent @event)
        {
            if (@event.SelectedMessage == null)
            {
                SelectedMessage = null;
            }
        }

        public void Handle(MessageBodyLoadedEvent @event)
        {
            SelectedMessage = @event.Message;
        }
    }
}