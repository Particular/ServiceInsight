using Caliburn.PresentationFramework.Screens;

namespace NServiceBus.Profiler.Desktop.MessageProperties
{
    public interface IErrorHeaderViewModel : IScreen
    {
        string ExceptionInfo { get; }
        string FailedQueue { get; }
        string TimeOfFailure { get; }

        void ReturnToSource();
        bool CanReturnToSource();
    }
}