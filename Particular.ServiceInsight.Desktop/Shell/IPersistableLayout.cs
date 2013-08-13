using Particular.ServiceInsight.Desktop.Core.Settings;

namespace Particular.ServiceInsight.Desktop.Shell
{
    public interface IPersistableLayout
    {
        void OnSaveLayout(ISettingsProvider settingsProvider);
        void OnRestoreLayout(ISettingsProvider settingsProvider);
        void OnResetLayout(ISettingsProvider settingsProvider);
    }
}