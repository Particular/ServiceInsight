namespace NServiceBus.Profiler.Desktop.Core.Licensing
{
    public interface ILicenseManager
    {
        ProfilerLicense CurrentLicense { get; }
        void Initialize(string license = null);
        int GetRemainingTrialDays();
        bool TrialExpired { get; }
    }
}