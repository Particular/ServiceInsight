namespace ServiceInsight.ValueConverters
{
    using System;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class CharacterCasingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            if (s == null)
            {
                return value;
            }

            if (!Enum.TryParse(parameter as string, out CharacterCasing casing))
            {
                casing = CharacterCasing.Upper;
            }

            switch (casing)
            {
                case CharacterCasing.Lower:
                    return s.ToLower(culture);

                case CharacterCasing.Upper:
                    return s.ToUpper(culture);

                default:
                    return s;
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}