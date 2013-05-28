namespace NServiceBus.Profiler.Desktop.Shell
{
    public interface IStatusBarManager
    {
        string StatusMessage { get; set; }
        string Registration { get; set; }
    }
}