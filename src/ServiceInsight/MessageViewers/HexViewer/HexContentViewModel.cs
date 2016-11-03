namespace ServiceInsight.MessageViewers.HexViewer
{
    using Caliburn.Micro;
    using ServiceInsight.Models;

    public class HexContentViewModel : Screen, IDisplayMessageBody
    {
        public MessageBody SelectedMessage { get; set; }

        protected override void OnActivate()
        {
            base.OnActivate();
            DisplayName = "Hex";
        }

        public void Display(StoredMessage selectedMessage)
        {
            SelectedMessage = selectedMessage;
        }
    }
}