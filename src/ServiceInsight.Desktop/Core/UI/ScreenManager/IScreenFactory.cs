namespace Particular.ServiceInsight.Desktop.Core.UI.ScreenManager
{
    public interface IScreenFactory
    {
        T CreateScreen<T>() where T : class;
    }
}