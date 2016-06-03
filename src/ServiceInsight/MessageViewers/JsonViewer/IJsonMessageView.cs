namespace ServiceInsight.MessageViewers.JsonViewer
{
    public interface IJsonMessageView
    {
        void Display(string message);

        void Clear();
    }
}