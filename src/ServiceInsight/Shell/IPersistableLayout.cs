namespace ServiceInsight.Shell
{
    using ServiceInsight.Framework.Settings;

    public interface IPersistableLayout
    {
        void OnSaveLayout(ISettingsProvider settingsProvider);
        void OnRestoreLayout(ISettingsProvider settingsProvider);
        void OnResetLayout(ISettingsProvider settingsProvider);
    }
}