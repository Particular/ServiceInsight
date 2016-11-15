namespace ServiceInsight.MessageViewers.JsonViewer
{
    using Caliburn.Micro;
    using ServiceInsight.Models;

    public class JsonMessageViewModel : Screen, IDisplayMessageBody
    {
        IJsonMessageView messageView;

        protected override void OnActivate()
        {
            base.OnActivate();
            DisplayName = "Json";
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            messageView = (IJsonMessageView)view;
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
            SelectedMessage = selectedMessage;
        }

        public void Clear()
        {
            messageView?.Clear();
        }
    }
}