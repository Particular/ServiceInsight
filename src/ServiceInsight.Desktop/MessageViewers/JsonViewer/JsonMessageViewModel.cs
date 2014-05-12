namespace Particular.ServiceInsight.Desktop.MessageViewers.JsonViewer
{
    using Caliburn.PresentationFramework.Screens;
    using Events;
    using Models;

    public class JsonMessageViewModel : Screen, IJsonMessageViewModel
    {
        private IJsonMessageView messageView;

        protected override void OnActivate()
        {
            base.OnActivate();
            DisplayName = "Json";
        }

        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            messageView = (IJsonMessageView)view;
            OnSelectedMessageChanged();
        }

        public MessageBody SelectedMessage { get; set; }

        public void OnSelectedMessageChanged()
        {
            if (messageView == null) return;

            messageView.Clear();

            if (SelectedMessage == null || SelectedMessage.Body == null) return;

            messageView.Display(SelectedMessage.Body);
        }

        public void Handle(SelectedMessageChanged @event)
        {
            SelectedMessage = @event.Message;
        }
    }
}