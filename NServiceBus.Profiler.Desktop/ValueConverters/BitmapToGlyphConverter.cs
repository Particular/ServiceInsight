using System;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using NServiceBus.Profiler.Desktop.ExtensionMethods;

namespace NServiceBus.Profiler.Desktop.ValueConverters
{
    public class BitmapToGlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bitmap = value as Bitmap;
            return bitmap != null ? bitmap.ToBitmapImage() : DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}