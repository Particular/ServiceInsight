using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace NServiceBus.Profiler.Core.Settings
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

        public string SerializeList(List<string> listOfItems)
        {
            return string.Empty;
        }

        public List<string> DeserializeList(string serializedList)
        {
            return new List<string>();
        }

        public void Save(string key, Dictionary<string, string> settings)
        {
            RegistryKey settingsRoot = null;

            try
            {
                using (var registry = RegistryKey.OpenBaseKey(_root, RegistryView.Registry64))
                {
                    var permission = RegistryKeyPermissionCheck.ReadWriteSubTree;
                    var settingKey = _registryKey + key.Replace('.', '\\');

                    settingsRoot = registry.OpenSubKey(settingKey, true) ?? registry.CreateSubKey(settingKey, permission, RegistryOptions.None);

                    foreach (var setting in settings)
                    {
                        settingsRoot.SetValue(setting.Key, setting.Value, RegistryValueKind.String);
                    }
                }
            }
            finally
            {
                if (settingsRoot != null)
                {
                    settingsRoot.Close();
                    settingsRoot.Dispose();
                }
            }
        }

        public Dictionary<string, string> Load(string key)
        {
            var dictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            var settingKey = _registryKey + key.Replace('.', '\\');

            using (var registry = RegistryKey.OpenBaseKey(_root, RegistryView.Registry64))
            using (var settingsRoot = registry.OpenSubKey(settingKey))
            {
                if (settingsRoot != null)
                {
                    var valueNames = settingsRoot.GetValueNames();

                    foreach (var name in valueNames)
                    {
                        dictionary.Add(string.Format("{0}.{1}", key, name), settingsRoot.GetValue(name).ToString());
                    }
                }
            }

            return dictionary;
        }
    }
}