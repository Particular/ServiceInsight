namespace Particular.ServiceInsight.Desktop.Core.Settings
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    public class SettingsProvider : ISettingsProvider
    {
        private readonly ISettingsStorage _settingsRepository;

        public SettingsProvider(ISettingsStorage settingsRepository = null)
        {
            _settingsRepository = settingsRepository ?? new IsolatedStorageSettingsStore();
        }

        public T GetSettings<T>() where T : new()
        {
            try
            {
                if (_settingsRepository.HasSettings(GetKey<T>()))
                {
                    return LoadSettings<T>(ReadSettingMetadata<T>());
                }
                else
                {
                    return GetDefaultSettings<T>();
                }
            }
            catch
            {
                return GetDefaultSettings<T>();
            }
        }

        private T GetDefaultSettings<T>() where T : new()
        {
            var settingMetadata = ReadSettingMetadata<T>();
            var defaultSetting = new T();

            foreach (var metadata in settingMetadata)
            {
                if (metadata.DefaultValue != null)
                {
                    metadata.Write(defaultSetting, metadata.DefaultValue);
                }
            }

            return defaultSetting;
        }

        protected virtual T LoadSettings<T>(IList<SettingDescriptor> metadata) where T : new()
        {
            return _settingsRepository.Load<T>(GetKey<T>(), metadata);
        }

        protected string GetKey<T>()
        {
            var type = typeof(T);

            var clazz = GetSettingTypeName(type.FullName);
            
            return clazz;
        }

        public virtual void SaveSettings<T>(T settings)
        {
            var settingsMetadata = ReadSettingMetadata<T>();

            foreach (var setting in settingsMetadata)
            {
                var value = setting.ReadValue(settings) ?? setting.DefaultValue;
                setting.Write(settings, value);
            }

            _settingsRepository.Save(GetKey<T>(), settings);
        }

        public virtual IList<SettingDescriptor> ReadSettingMetadata<T>()
        {
            return ReadSettingMetadata(typeof(T));
        }

        public virtual IList<SettingDescriptor> ReadSettingMetadata(Type settingsType)
        {
            return settingsType.GetProperties()
                .Where(x => x.CanRead && x.CanWrite)
                .Select(x => new SettingDescriptor(x))
                .ToArray();
        }

        private static string GetSettingTypeName(string name)
        {
            var namespaceSeparator = name.LastIndexOf('.');
            var internalClassName = name.IndexOf('+');
            string settingName;
            if (namespaceSeparator > 0)
            {
                if (internalClassName > 0)
                {
                    settingName = name.Substring(namespaceSeparator + 1).Replace('+', '.');
                }
                else
                {
                    settingName = name.Substring(namespaceSeparator + 1);
                }
            }
            else
            {
                settingName = name;
            }

            return settingName.EndsWith("Settings") ? settingName.Replace("Settings", string.Empty) : settingName;
        }
    }

    public class SettingDescriptor
    {
        public SettingDescriptor(PropertyInfo property)
        {
            Property = property;
            DisplayName = property.Name;

            ReadAttribute<DefaultValueAttribute>(d => DefaultValue = d.Value);
            ReadAttribute<DescriptionAttribute>(d => Description = d.Description);
            ReadAttribute<DisplayNameAttribute>(d => DisplayName = d.DisplayName);
        }

        private void ReadAttribute<TAttribute>(Action<TAttribute> callback)
        {
            var instances = Property.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>();
            foreach (var instance in instances)
            {
                callback(instance);
            }
        }

        public PropertyInfo Property { get; private set; }

        public object DefaultValue { get; private set; }

        public string Description { get; private set; }

        public string DisplayName { get; private set; }

        public void Write(object settings, object value)
        {
            Property.SetValue(settings, value, null);
        }

        public object ReadValue(object settings)
        {
            return Property.GetValue(settings, null);
        }
    }

}
