namespace Particular.ServiceInsight.Desktop.MessageViewers.XmlViewer
{
    public interface IXmlMessageView
    {
        void Display(string message);
        void Clear();
    }
}