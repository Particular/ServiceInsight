namespace NServiceBus.Profiler.Desktop.MessageProperties
{
    public interface IErrorHeaderViewModel : IPropertyDataProvider
    {
        string ExceptionInfo { get; }
        string FailedQueue { get; }
        string TimeOfFailure { get; }

        void ReturnToSource();
        bool CanReturnToSource();
    }
}