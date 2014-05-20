namespace Particular.ServiceInsight.Desktop.ValueConverters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class BoolToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = false;
            if (value is bool)
            {
                flag = (bool)value;
            }
            else
            {
                if (value is bool?)
                {
                    bool? flag2 = (bool?)value;
                    flag = (flag2.HasValue && flag2.Value);
                }
            }
            return (flag ^ Invert) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = false;

            if (value is Visibility)
            {
                result = (Visibility)value == Visibility.Visible;
            }

            return result ^ Invert;
        }
    }
}