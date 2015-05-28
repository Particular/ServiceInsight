namespace Particular.ServiceInsight.Desktop.ValueConverters
{
    using System;
    using System.Text;
    using System.Windows;
    using System.Windows.Data;
    using Particular.ServiceInsight.Desktop.ExtensionMethods;

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

    public class ByteToCharConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is byte)
            {
                Func<byte, string> ByteToStringConverter = byteValue => Encoding.UTF8.GetString(new[] { byteValue });
                var c = ByteToStringConverter.TryGetValue((byte)value, " ");
                if (c == "\r" || c == "\n" || c == "\t")
                {
                    return ".";
                }

                return c;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TimeSpanHumanizedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is TimeSpan)
            {
                return ((TimeSpan)value).SubmillisecondHumanize();
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}