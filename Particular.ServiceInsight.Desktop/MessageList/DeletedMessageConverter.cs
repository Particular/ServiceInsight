using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Particular.ServiceInsight.Desktop.MessageList
{
    public class DeletedMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (true.Equals(value))
            {
                return TextDecorations.Strikethrough;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}