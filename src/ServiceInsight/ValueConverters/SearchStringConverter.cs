namespace ServiceInsight.ValueConverters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class SearchStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                return Binding.DoNothing;
            }

            var stringValue = value as string;
            if (stringValue == null)
            {
                return DependencyProperty.UnsetValue;
            }

            return stringValue.StartsWith("\"", true, culture) && stringValue.EndsWith("\"", true, culture)
                ? stringValue.Substring(1, stringValue.Length - 2)
                : stringValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            string.Format(culture, "\"{0}\"", value);
    }
}