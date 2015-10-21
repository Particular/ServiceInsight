namespace ServiceInsight.SequenceDiagram.Converter
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class HeightToLinePoinConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double height;
            double.TryParse(value.ToString(), out height);

            return new Point(0, height);
        }
    }
}