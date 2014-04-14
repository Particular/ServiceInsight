namespace NServiceBus.Profiler.Desktop.Startup
{
    public interface IEnvironment
    {
        string[] GetCommandLineArgs();
    }
}