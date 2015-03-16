namespace Particular.ServiceInsight.Desktop.ValueConverters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    public class BoolToStrokeDashArrayConverter : IValueConverter
    {
        private static DoubleCollection emptyStrokeDashArray = new DoubleCollection();

        public bool Invert { get; set; }
        public DoubleCollection StrokeDashArray { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var flag = false;
            if (value is bool)
            {
                flag = (bool)value;
            }
            return (flag ^ Invert) ? StrokeDashArray : emptyStrokeDashArray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}