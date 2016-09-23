namespace ServiceInsight.ValueConverters
{
    using System;
    using System.Windows.Data;

    public class ScaledValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double scalingFactor = 0;
            if (parameter != null)
            {
                double.TryParse((string)parameter, out scalingFactor);
            }

            if (Math.Abs(scalingFactor) < double.Epsilon)
            {
                return double.NaN;
            }

            return (double)value * scalingFactor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}