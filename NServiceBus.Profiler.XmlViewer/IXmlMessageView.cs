namespace NServiceBus.Profiler.XmlViewer
{
    public interface IXmlMessageView// : IPluginDiscoverablePart
    {
        void Display(string message);
        void Clear();
    }
}