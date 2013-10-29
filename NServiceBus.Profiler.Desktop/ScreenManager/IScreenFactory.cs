namespace NServiceBus.Profiler.Desktop.ScreenManager
{
    public interface IScreenFactory
    {
        T CreateScreen<T>() where T : class;
    }
}