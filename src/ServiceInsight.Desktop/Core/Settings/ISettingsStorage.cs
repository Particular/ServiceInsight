using System.Collections.Generic;

namespace NServiceBus.Profiler.Desktop.Core.Settings
{
    public interface ISettingsStorage
    {
        void Save<T>(string key, T settings);
        T Load<T>(string key, IList<SettingDescriptor> metadata) where T : new();
        bool HasSettings(string key);
    }
}