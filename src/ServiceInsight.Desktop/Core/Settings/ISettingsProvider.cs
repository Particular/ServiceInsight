namespace Particular.ServiceInsight.Desktop.Core.Settings
{
    using System;
    using System.Collections.Generic;

    public interface ISettingsProvider
    {
        T GetSettings<T>() where T : new();
        void SaveSettings<T>(T settings);
        IList<SettingDescriptor> ReadSettingMetadata<T>();
        IList<SettingDescriptor> ReadSettingMetadata(Type settingsType);
    }
}