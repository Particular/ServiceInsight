namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    public interface IGeneralHeaderDisplay
    {
        bool CanCopyHeaderInfo();
        void CopyHeaderInfo();
    }
}