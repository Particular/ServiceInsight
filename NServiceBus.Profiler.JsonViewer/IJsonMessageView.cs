namespace NServiceBus.Profiler.JsonViewer
{
    public interface IJsonMessageView
    {
        void Display(string message);
        void Clear();
    }
}