using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Particular.ServiceInsight.Desktop.MessageList
{
    public class FileSizeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;

            var provider = new FileSizeFormatProvider();
            return provider.Format("fs", value, provider);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}