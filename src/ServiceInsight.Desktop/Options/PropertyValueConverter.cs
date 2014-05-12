namespace Particular.ServiceInsight.Desktop.Options
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Data;

    public class PropertyValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;

            return from p in value.GetType().GetProperties()
                   where IsEditableProperty(p)
                   select new OptionPropertyValue(p, value);
        }

        bool IsEditableProperty(MemberInfo propertyInfo)
        {
            var browsable = propertyInfo.GetCustomAttribute<EditorBrowsableAttribute>();
            return browsable == null || browsable.State != EditorBrowsableState.Never;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}