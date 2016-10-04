namespace ServiceInsight.MessageViewers
{
    public interface IMessageView
    {
        void Display(string message);

        void Clear();
    }
}