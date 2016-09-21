namespace ServiceInsight.MessageViewers.HexViewer
{
    using Framework.Rx;
    using ServiceInsight.Models;

    public class HexContentViewModel : RxScreen, IDisplayMessageBody
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