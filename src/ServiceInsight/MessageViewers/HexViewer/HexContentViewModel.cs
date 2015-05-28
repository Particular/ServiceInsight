namespace Particular.ServiceInsight.Desktop.MessageViewers.HexViewer
{
    using Caliburn.Micro;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using Particular.ServiceInsight.Desktop.Models;

    public class HexContentViewModel : Screen, IHandle<SelectedMessageChanged>
    {
        public MessageBody SelectedMessage { get; set; }

        protected override void OnActivate()
        {
            base.OnActivate();
            DisplayName = "Hex";
        }

        public void Handle(SelectedMessageChanged @event)
        {
            SelectedMessage = @event.Message;
        }
    }
}