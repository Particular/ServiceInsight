namespace ServiceInsight.ValueConverters
{
    using System;
    using System.Text;
    using System.Windows;
    using System.Windows.Data;
    using ServiceInsight.ExtensionMethods;

    public class ByteToCharConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is byte)
            {
                Func<byte, string> byteToStringConverter = byteValue => Encoding.UTF8.GetString(new[] { byteValue });
                var c = byteToStringConverter.TryGetValue((byte)value, " ");
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
            throw new NotSupportedException();
        }
    }
}