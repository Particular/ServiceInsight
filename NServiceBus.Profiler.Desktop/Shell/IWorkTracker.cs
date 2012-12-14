namespace NServiceBus.Profiler.Desktop.Shell
{
    public interface IWorkTracker
    {
        bool WorkInProgress { get; }
    }
}