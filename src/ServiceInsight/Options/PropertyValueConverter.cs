namespace ServiceInsight.Options
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Data;
    using Anotar.Serilog;

    public class PropertyValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Binding.DoNothing;
            }

            var properties = value.GetType().GetProperties();

            var editableProperties = properties
                .Where(IsEditableProperty)
                .Select(p => new OptionPropertyValue(p, value, properties.FirstOrDefault(p2 => p2.Name == "Default" + p.Name)))
                .ToList();

            return editableProperties;
        }

        bool IsEditableProperty(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanRead)
            {
                LogTo.Debug("Property {Name} of {DeclaringType} is not readable.", propertyInfo.Name, propertyInfo.DeclaringType);
                return false;
            }

            if (!propertyInfo.CanWrite)
            {
                LogTo.Debug("Property {Name} of {DeclaringType} is not writable.", propertyInfo.Name, propertyInfo.DeclaringType);
                return false;
            }

            if (propertyInfo.GetCustomAttribute<DisplayNameAttribute>() == null)
            {
                LogTo.Debug("Property {Name} of {DeclaringType} does not have DisplayNameAttribute.", propertyInfo.Name, propertyInfo.DeclaringType);
                return false;
            }

            var browsable = propertyInfo.GetCustomAttribute<EditorBrowsableAttribute>();
            return browsable == null || browsable.State != EditorBrowsableState.Never;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }
}