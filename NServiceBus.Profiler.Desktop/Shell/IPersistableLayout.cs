using NServiceBus.Profiler.Desktop.Core.Settings;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public interface IPersistableLayout
    {
        void SaveLayout(ISettingsProvider settingsProvider);
        void RestoreLayout(ISettingsProvider settingsProvider);
        void ResetLayout(ISettingsProvider settingsProvider);
    }
}