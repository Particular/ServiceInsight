using System;
using System.ComponentModel;
using System.Reflection;

namespace Particular.ServiceInsight.Desktop.Options
{
    public class OptionPropertyValue : IDataErrorInfo
    {
        private readonly PropertyInfo _propertyInfo;
        private readonly object _owner;
        private readonly DisplayNameAttribute _displayName;
        private readonly DescriptionAttribute _description;

        public OptionPropertyValue(PropertyInfo propertyInfo, object owner)
        {
            _propertyInfo = propertyInfo;
            _owner = owner;
            _displayName = _propertyInfo.GetCustomAttribute<DisplayNameAttribute>();
            _description = _propertyInfo.GetCustomAttribute<DescriptionAttribute>();
        }

        public string Name
        {
            get
            {
                return _displayName != null ? _displayName.DisplayName : _propertyInfo.Name;
            }
        }

        public string Description
        {
            get
            {
                return _description == null ? string.Empty : _description.Description;
            }
        }

        public Type PropertyType
        {
            get { return _propertyInfo.PropertyType; }
        }

        public object Value
        {
            get { return _propertyInfo.GetValue(_owner, null); }
            set { TrySetValue(value); }
        }

        private void TrySetValue(object value)
        {
            var convertedValue = Convert.ChangeType(value, PropertyType);
            _propertyInfo.SetValue(_owner, convertedValue, null);
        }

        public string this[string columnName]
        {
            get { return string.Empty; }
        }

        public string Error { get; private set; }
    }
}