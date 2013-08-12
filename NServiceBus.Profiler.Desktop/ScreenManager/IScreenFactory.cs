using Caliburn.PresentationFramework.Screens;

namespace Particular.ServiceInsight.Desktop.ScreenManager
{
    public interface IScreenFactory
    {
        T CreateScreen<T>() where T : IScreen;
    }
}