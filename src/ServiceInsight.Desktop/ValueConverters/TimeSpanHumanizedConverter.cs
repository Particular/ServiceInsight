namespace Particular.ServiceInsight.Desktop.ValueConverters
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using Humanizer;

    public class TimeSpanHumanizedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is TimeSpan)
            {
                return ((TimeSpan)value).Humanize();
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}