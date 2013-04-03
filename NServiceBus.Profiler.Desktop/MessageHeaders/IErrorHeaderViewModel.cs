using Caliburn.PresentationFramework.Screens;

namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    public interface IErrorHeaderViewModel : IScreen
    {
        void ReturnToSource();
        bool CanReturnToSource();
    }
}