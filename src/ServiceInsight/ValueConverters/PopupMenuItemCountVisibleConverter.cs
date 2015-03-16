namespace Particular.ServiceInsight.Desktop.ValueConverters
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    public class PopupMenuItemCountVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var count = (int)value;

            return count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}