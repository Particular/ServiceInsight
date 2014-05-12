namespace Particular.ServiceInsight.Desktop.Options
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    public class OptionPropertyValue : IDataErrorInfo
    {
        private readonly PropertyInfo propertyInfo;
        private readonly object owner;
        private readonly DisplayNameAttribute displayName;
        private readonly DescriptionAttribute description;

        public OptionPropertyValue(PropertyInfo propertyInfo, object owner)
        {
            this.propertyInfo = propertyInfo;
            this.owner = owner;
            displayName = this.propertyInfo.GetCustomAttribute<DisplayNameAttribute>();
            description = this.propertyInfo.GetCustomAttribute<DescriptionAttribute>();
        }

        public string Name
        {
            get
            {
                return displayName != null ? displayName.DisplayName : propertyInfo.Name;
            }
        }

        public string Description
        {
            get
            {
                return description == null ? string.Empty : description.Description;
            }
        }

        public Type PropertyType
        {
            get { return propertyInfo.PropertyType; }
        }

        public object Value
        {
            get { return propertyInfo.GetValue(owner, null); }
            set { TrySetValue(value); }
        }

        private void TrySetValue(object value)
        {
            var convertedValue = Convert.ChangeType(value, PropertyType);
            propertyInfo.SetValue(owner, convertedValue, null);
        }

        public string this[string columnName]
        {
            get { return string.Empty; }
        }

        public string Error { get; private set; }
    }
}