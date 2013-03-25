namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    public interface IErrorHeaderDisplay
    {
        void ReturnToSource();
        bool CanReturnToSource();
    }
}