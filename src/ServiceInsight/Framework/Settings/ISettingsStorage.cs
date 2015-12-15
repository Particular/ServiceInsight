namespace ServiceInsight.Framework.Settings
{
    using System.Collections.Generic;

    public interface ISettingsStorage
    {
        void Save<T>(string key, T settings);
        T Load<T>(string key, IList<SettingDescriptor> metadata) where T : new();
        bool HasSettings(string key);
    }
}