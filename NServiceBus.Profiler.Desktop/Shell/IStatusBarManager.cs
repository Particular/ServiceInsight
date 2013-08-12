namespace Particular.ServiceInsight.Desktop.Shell
{
    public interface IStatusBarManager
    {
        string StatusMessage { get; }
        string Registration { get; }
        void SetRegistrationInfo(string message, params object[] args);
        void SetSuccessStatusMessage(string message, params object[] args);
        void SetFailStatusMessage(string message, params object[] args);
        void Done();
    }
}