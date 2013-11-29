using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NServiceBus.Profiler.Desktop.ValueConverters
{
    public class BoolToWindowBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool) value ? WindowStyle.None : WindowStyle.SingleBorderWindow;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}