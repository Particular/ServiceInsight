namespace Particular.ServiceInsight.Desktop.MessageViewers
{
    using Particular.ServiceInsight.Desktop.Models;

    public interface IDisplayMessageBody
    {
        void Display(StoredMessage selectedMessage);
    }
}