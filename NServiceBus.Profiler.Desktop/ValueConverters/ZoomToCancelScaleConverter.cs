using System;
using System.Windows.Data;
using System.Windows.Media;

namespace NServiceBus.Profiler.Desktop.ValueConverters
{
    public class ZoomToCancelScaleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double zoom = (double)value;
            return new ScaleTransform(1 / zoom, 1 / zoom);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}