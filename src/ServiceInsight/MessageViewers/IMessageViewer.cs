namespace ServiceInsight.MessageViewers
{
    public interface IMessageViewer
    {
        void Display(string message);

        void Clear();
    }
}