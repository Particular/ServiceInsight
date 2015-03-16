namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class EndpointThicknessConverter : IValueConverter
    {
        public double ColumnWidth { get; set; }

        public bool Middle { get; set; }

        public double Top { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var column = (int)value;

            return new Thickness(column * ColumnWidth + (Middle ? (ColumnWidth / 2) : 0), Top, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}