namespace ServiceInsight.ValueConverters
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    public class MiddlePointValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (values.Length != 2)
            {
                throw new ArgumentException("MiddlePointValueConverter (value)");
            }

            if (values[0] == DependencyProperty.UnsetValue ||
                values[1] == DependencyProperty.UnsetValue)
            {
                return 0.0;
            }

            var fromPoint = (double)values[0];
            var toPoint = (double)values[1];

            return fromPoint +
                   (toPoint - fromPoint) / 2;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) => null;
    }
}