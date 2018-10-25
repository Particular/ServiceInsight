namespace ServiceInsight.ValueConverters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using ServiceInsight.ExtensionMethods;
    using ServiceInsight.Framework.Licensing;

    public class DateStatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var app = Application.Current;

            if (value is DateExpirationStatus)
            {
                var status = (DateExpirationStatus)value;
                if (status == DateExpirationStatus.Expired ||
                    status == DateExpirationStatus.Expiring ||
                    status == DateExpirationStatus.ExpiringToday)
                {
                    return app?.TryFindResource("ErrorStatusBrush");
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}