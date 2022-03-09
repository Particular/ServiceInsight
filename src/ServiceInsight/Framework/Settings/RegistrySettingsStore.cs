namespace ServiceInsight.Framework.Settings
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Win32;

    public class RegistrySettingsStore : ISettingsStorage
    {
        RegistryHive root;
        string registryKey;

        public RegistrySettingsStore(string registryKey)
        {
            root = RegistryHive.LocalMachine;

            this.registryKey = registryKey;

            if (!this.registryKey.EndsWith("\\"))
            {
                this.registryKey += "\\";
            }
        }

        public void Save<T>(string key, T settings)
        {
            throw new NotSupportedException(); //Do we even need this??
        }

        public T Load<T>(string key, IList<SettingDescriptor> metadata) where T : new()
        {
            var dictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            var settingKey = registryKey + key.Replace('.', '\\');
            var setting = new T();

            using (var registry = RegistryKey.OpenBaseKey(root, RegistryView.Registry64))
            using (var settingsRoot = registry.OpenSubKey(settingKey))
            {
                if (settingsRoot != null)
                {
                    var valueNames = settingsRoot.GetValueNames();

                    foreach (var name in valueNames)
                    {
                        dictionary.Add(name, settingsRoot.GetValue(name).ToString());
                    }
                }
            }

            PopulateSetting(setting, metadata, dictionary);

            return setting;
        }

        void PopulateSetting(object setting, IList<SettingDescriptor> metadata, Dictionary<string, string> data)
        {
            foreach (var property in metadata)
            {
                var value = data.ContainsKey(property.Property.Name)
                    ? data[property.Property.Name]
                    : property.DefaultValue;

                var typedValue = TryCastOrDefault(value, property);

                if (typedValue != null)
                {
                    property.Write(setting, typedValue);
                }
            }
        }

        static object TryCastOrDefault(object value, SettingDescriptor property)
        {
            try
            {
                return Convert.ChangeType(value, property.Property.PropertyType);
            }
            catch
            {
                return property.DefaultValue;
            }
        }

        public bool HasSettings(string key)
        {
            var settingKey = registryKey + key.Replace('.', '\\');
            using (var registry = RegistryKey.OpenBaseKey(root, RegistryView.Registry64))
            {
                return registry.OpenSubKey(settingKey) != null;
            }
        }
    }
}