namespace ServiceInsight.MessageViewers.XmlViewer
{
    using Caliburn.Micro;
    using ServiceInsight.Models;

    public class XmlMessageViewModel : Screen, IDisplayMessageBody
    {
        IXmlMessageView messageView;

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
            if (messageView == null)
            {
                return;
            }

            messageView.Clear();

            if (SelectedMessage == null || SelectedMessage.Body == null)
            {
                return;
            }

            messageView.Display(SelectedMessage.Body.Text);
        }

        public void Display(StoredMessage selectedMessage)
        {
            if (SelectedMessage == selectedMessage) //Workaround, to force refresh the property. Should refactor to use the same approach as hex viewer.
            {
                OnSelectedMessageChanged();
            }
            else
            {
                SelectedMessage = selectedMessage;
            }
        }
    }
}