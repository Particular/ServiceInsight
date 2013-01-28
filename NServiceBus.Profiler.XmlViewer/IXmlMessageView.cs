namespace NServiceBus.Profiler.XmlViewer
{
    public interface IXmlMessageView
    {
        void Display(string message);
        void Clear();
    }
}