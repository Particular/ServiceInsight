using System.Collections.Generic;

namespace Particular.ServiceInsight.Desktop.Core.Settings
{
    public interface ISettingsStorage
    {
        void Save<T>(string key, T settings);
        T Load<T>(string key, IList<SettingDescriptor> metadata) where T : new();
    }
}