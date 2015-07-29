namespace Particular.ServiceInsight.Desktop.MessageViewers.XmlViewer
{
    using Caliburn.Micro;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using Particular.ServiceInsight.Desktop.Models;

    public class XmlMessageViewModel : Screen,
        IHandle<SelectedMessageChanged>
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
            if (messageView == null) return;

            messageView.Clear();

            if (SelectedMessage == null || SelectedMessage.Body == null)
            {
                return;
            }

            messageView.Display(SelectedMessage.Body.Text);
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
    }
}