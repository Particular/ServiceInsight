namespace ServiceInsight.ValueConverters
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    public class ByteToHexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is byte)
            {
                return string.Format((byte)value < 0x10 ? "0{0:X000} " : "{0:X000} ", value);
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}