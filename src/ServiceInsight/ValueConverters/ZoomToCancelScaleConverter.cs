namespace ServiceInsight.ValueConverters
{
    using System;
    using System.Windows.Data;
    using System.Windows.Media;

    public class ZoomToCancelScaleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var zoom = (double)value;
            return new ScaleTransform(1 / zoom, 1 / zoom);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}