namespace Particular.ServiceInsight.Desktop.MessageViewers.HexViewer
{
    using System.Text;
    using Caliburn.Micro;
    using Particular.ServiceInsight.Desktop.Framework.Events;

    public class HexContentViewModel : Screen, IHandle<SelectedMessageChanged>
    {
        public byte[] SelectedMessage { get; set; }

        protected override void OnActivate()
        {
            base.OnActivate();
            DisplayName = "Hex";
        }

        public void Handle(SelectedMessageChanged @event)
        {
            byte[] body = null;

            if (@event.Message != null && @event.Message.Body != null)
            {
                body = Encoding.Default.GetBytes(@event.Message.Body);
            }

            SelectedMessage = body;
        }
    }
}