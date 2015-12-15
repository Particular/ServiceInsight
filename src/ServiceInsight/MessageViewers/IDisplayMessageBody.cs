namespace ServiceInsight.MessageViewers
{
    using ServiceInsight.Models;

    public interface IDisplayMessageBody
    {
        void Display(StoredMessage selectedMessage);
    }
}