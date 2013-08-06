using NServiceBus.Profiler.Desktop.Core.Settings;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public interface IPersistableLayout
    {
        void OnSaveLayout(ISettingsProvider settingsProvider);
        void OnRestoreLayout(ISettingsProvider settingsProvider);
        void OnResetLayout(ISettingsProvider settingsProvider);
    }
}