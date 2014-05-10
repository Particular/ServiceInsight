namespace Particular.ServiceInsight.Desktop.Shell
{
    using Core.Settings;

    public interface IPersistableLayout
    {
        void OnSaveLayout(ISettingsProvider settingsProvider);
        void OnRestoreLayout(ISettingsProvider settingsProvider);
        void OnResetLayout(ISettingsProvider settingsProvider);
    }
}