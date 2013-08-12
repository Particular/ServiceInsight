using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Particular.ServiceInsight.Desktop.Core.Settings
{
    public class RegistrySettingsStore : ISettingsStorage
    {
        private readonly RegistryHive _root;
        private readonly string _registryKey;

        public RegistrySettingsStore(string registryKey)
        {
            _root = RegistryHive.LocalMachine;

            _registryKey = registryKey;

            if (!_registryKey.EndsWith("\\"))
                _registryKey += "\\";
        }

        public void Save<T>(string key, T settings)
        {
            throw new NotImplementedException(); //Do we even need this??
        }

        public T Load<T>(string key, IList<SettingDescriptor> metadata) where T : new()
        {
            var dictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            var settingKey = _registryKey + key.Replace('.', '\\');
            var setting = new T();

            using (var registry = RegistryKey.OpenBaseKey(_root, RegistryView.Registry64))
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

        private void PopulateSetting(object setting, IList<SettingDescriptor> metadata, Dictionary<string, string> data)
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

        private static object TryCastOrDefault(object value, SettingDescriptor property)
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
    }
}