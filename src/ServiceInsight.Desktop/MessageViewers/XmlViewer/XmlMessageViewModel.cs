namespace Particular.ServiceInsight.Desktop.MessageViewers.XmlViewer
{
    using System.Text;
    using System.Xml;
    using Caliburn.Micro;
    using Core.MessageDecoders;
    using Events;
    using ExtensionMethods;
    using Models;

    public class XmlMessageViewModel : Screen,
        IHandle<SelectedMessageChanged>
    {
        IContentDecoder<XmlDocument> xmlDecoder;
        IXmlMessageView messageView;

        public XmlMessageViewModel(
            IContentDecoder<XmlDocument> xmlDecoder)
        {
            this.xmlDecoder = xmlDecoder;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            DisplayName = "Xml";
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            messageView = (IXmlMessageView)view;
            OnSelectedMessageChanged();
        }

        public MessageBody SelectedMessage { get; set; }

        public void OnSelectedMessageChanged()
        {
            if (messageView == null) return;

            messageView.Clear();
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
                AppServices.Clipboard.CopyTo(content);
            }
        }

        public void Handle(SelectedMessageChanged @event)
        {
            if (SelectedMessage == @event.Message) //Workaround, to force refresh the property. Should refactor to use the same approach as hex viewer.
            {
                OnSelectedMessageChanged();
            }
            else
            {
                SelectedMessage = @event.Message;
            }
        }

        void ShowMessageBody()
        {
            if (SelectedMessage == null) return;
            messageView.Display(GetMessageBody());
        }

        string GetMessageBody()
        {
            if (SelectedMessage == null || SelectedMessage.Body == null) return string.Empty;

            var bytes = Encoding.Default.GetBytes(SelectedMessage.Body);
            var xml = xmlDecoder.Decode(bytes);
            return xml.IsParsed ? xml.Value.GetFormatted() : string.Empty;
        }
    }
}