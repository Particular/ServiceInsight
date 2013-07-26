using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace NServiceBus.Profiler.Desktop.Core.Settings
{
    public class SettingsProvider : ISettingsProvider
    {
        private const string NotConvertableMessage = "Settings provider only supports types that Convert.ChangeType supports. See http://msdn.microsoft.com/en-us/library/dtb69x08.aspx";
        private readonly ISettingsStorage _settingsRepository;
        private readonly Dictionary<Type, object> _cache = new Dictionary<Type, object>();

        public SettingsProvider(ISettingsStorage settingsRepository = null)
        {
            _settingsRepository = settingsRepository ?? new IsolatedStorageSettingsStore();
        }

        public virtual T GetSettings<T>(bool fresh = false) where T : new()
        {
            var type = typeof (T);
            if (!fresh && _cache.ContainsKey(type))
                return (T)_cache[type];

            var settingsLookup = LoadSettings<T>();
            var settings = new T();
            var settingMetadata = ReadSettingMetadata<T>();

            foreach (var setting in settingMetadata)
            {
                // Write over it using the stored value if exists
                var key = GetKey<T>(setting);
                var value = settingsLookup.ContainsKey(key) ? ConvertValue(settingsLookup[key], setting) 
                                                            : GetDefaultValue(setting);
                setting.Write(settings, value);
            }

            _cache[typeof(T)] = settings;

            return settings;
        }

        protected virtual Dictionary<string, string> LoadSettings<T>()
        {
            return _settingsRepository.Load(GetKey<T>());
        }

        private object GetDefaultValue(SettingDescriptor setting)
        {
            return setting.DefaultValue ?? ConvertValue(null, setting);
        }

        protected string GetKey<T>()
        {
            var type = typeof(T);

            var clazz = GetSettingTypeName(type.FullName);
            
            return clazz;
        }

        private object ConvertValue(string storedValue, SettingDescriptor setting)
        {
            return ConvertValue(storedValue, setting.UnderlyingType);
        }

        private object ConvertValue(string storedValue, Type underlyingType)
        {
            if (underlyingType == typeof(string)) return storedValue;
            var isList = IsList(underlyingType);
            if (isList && string.IsNullOrEmpty(storedValue)) return CreateListInstance(underlyingType);
            if (underlyingType != typeof(string) && string.IsNullOrEmpty(storedValue)) return null;
            if (underlyingType.IsEnum) return Enum.Parse(underlyingType, storedValue, false);
            if (underlyingType == typeof(Guid)) return Guid.Parse(storedValue);
            if (isList) return ReadList(storedValue, underlyingType);

            object converted;
            try
            {
                converted = Convert.ChangeType(storedValue, underlyingType, CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException ex)
            {
                throw new NotSupportedException(NotConvertableMessage, ex);
            }
            catch (FormatException ex)
            {
                throw new NotSupportedException(NotConvertableMessage, ex);
            }

            return converted;
        }

        private object ReadList(string storedValue, Type propertyType)
        {
            var listItemType = propertyType.GetGenericArguments()[0];
            var list = CreateListInstance(propertyType);
            var listInterface = (IList)list;

            var valueList = _settingsRepository.DeserializeList(storedValue);

            foreach (var value in valueList)
            {
                listInterface.Add(ConvertValue(value, listItemType));
            }

            return list;
        }

        private static object CreateListInstance(Type propertyType)
        {
            return Activator.CreateInstance(propertyType.IsClass ? propertyType : typeof(List<>).MakeGenericType(propertyType.GetGenericArguments()[0]));
        }

        private static bool IsList(Type propertyType)
        {
            return typeof(IList).IsAssignableFrom(propertyType) ||
                   (propertyType.IsGenericType && typeof(IList<>) == propertyType.GetGenericTypeDefinition());
        }

        public virtual void SaveSettings<T>(T settingsToSave)
        {
            _cache[typeof (T)] = settingsToSave;

            var settings = new Dictionary<string, string>();
            var settingsMetadata = ReadSettingMetadata<T>();

            foreach (var setting in settingsMetadata)
            {
                var value = setting.ReadValue(settingsToSave) ?? setting.DefaultValue;
                if (value == null && setting.UnderlyingType.IsEnum)
                    value = EnumHelper.GetValues(setting.UnderlyingType).First();
                if (IsList(setting.UnderlyingType) && value != null)
                    settings[GetKey<T>(setting)] = WriteList(value);
                else
                    settings[GetKey<T>(setting)] = Convert.ToString(value ?? string.Empty, CultureInfo.InvariantCulture);
            }
            _settingsRepository.Save(GetKey<T>(), settings);
        }

        private string WriteList(object value)
        {
            var list = (from object item in (IList)value
                        select Convert.ToString(item ?? string.Empty, CultureInfo.CurrentCulture)).ToList();

            return _settingsRepository.SerializeList(list);
        }

        internal static string GetKey<T>(SettingDescriptor setting)
        {
            var settingsType = GetSettingTypeName(typeof(T).FullName);

            return string.Format("{0}.{1}", settingsType, setting.Property.Name);
        }

        public virtual IEnumerable<SettingDescriptor> ReadSettingMetadata<T>()
        {
            return ReadSettingMetadata(typeof(T));
        }

        public virtual IEnumerable<SettingDescriptor> ReadSettingMetadata(Type settingsType)
        {
            return settingsType.GetProperties()
                .Where(x => x.CanRead && x.CanWrite)
                .Select(x => new SettingDescriptor(x))
                .ToArray();
        }

        public virtual T ResetToDefaults<T>() where T : new()
        {
            _settingsRepository.Save(GetKey<T>(), new Dictionary<string, string>());

            var type = typeof (T);
            if (_cache.ContainsKey(type))
            {
                var cachedCopy = _cache[type];
                var settingMetadata = ReadSettingMetadata<T>();

                foreach (var setting in settingMetadata)
                {
                    setting.Write(cachedCopy, GetDefaultValue(setting));
                }

                return (T)cachedCopy;
            }

            return GetSettings<T>();
        }

        private static string GetSettingTypeName(string name)
        {
            var namespaceSeparator = name.LastIndexOf('.');
            var internalClassName = name.IndexOf('+');
            var settingName = string.Empty;
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

            return settingName;
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

            void ReadAttribute<TAttribute>(Action<TAttribute> callback)
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

            /// <summary>
            /// If the property type is nullable, returns the type. i.e int? returns int
            /// </summary>
            public Type UnderlyingType
            {
                get
                {
                    if (Property.PropertyType.IsGenericType && Property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        return Property.PropertyType.GetGenericArguments()[0];
                    return Property.PropertyType;
                }
            }

            public object ReadValue(object settings)
            {
                return Property.GetValue(settings, null);
            }
        }
    }
}
