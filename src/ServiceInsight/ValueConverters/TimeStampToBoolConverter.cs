namespace ServiceInsight.ValueConverters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class TimeStampToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ts = value as TimeSpan?;
            if (ts.HasValue && ts.Value.Ticks >= 0)
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}