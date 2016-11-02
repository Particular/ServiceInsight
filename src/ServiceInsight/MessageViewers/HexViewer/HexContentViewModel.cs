namespace ServiceInsight.MessageViewers.HexViewer
{
    using Framework.Rx;
    using ServiceInsight.Models;

    public class HexContentViewModel : RxScreen, IDisplayMessageBody
    {
        public HexContentViewModel()
        {
            DisplayName = "Hex";
        }

        public MessageBody SelectedMessage { get; set; }

        public void Display(StoredMessage selectedMessage)
        {
            SelectedMessage = selectedMessage;
        }
    }
}