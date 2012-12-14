using Caliburn.PresentationFramework.Screens;

namespace NServiceBus.Profiler.Desktop.ScreenManager
{
    public interface IScreenFactory
    {
        T CreateScreen<T>() where T : IScreen;
    }
}