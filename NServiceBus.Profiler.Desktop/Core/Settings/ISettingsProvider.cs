using System;
using System.Collections.Generic;

namespace NServiceBus.Profiler.Desktop.Core.Settings
{
    public interface ISettingsProvider
    {
        T GetSettings<T>(bool freshCopy = false) where T : new();
        void SaveSettings<T>(T settings);
        IEnumerable<SettingsProvider.SettingDescriptor> ReadSettingMetadata<T>();
        IEnumerable<SettingsProvider.SettingDescriptor> ReadSettingMetadata(Type settingsType);
        T ResetToDefaults<T>() where T : new();
    }
}