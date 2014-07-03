using System;
using System.Globalization;

namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System.Windows.Data;

    public class EndpointPositionConverter : IValueConverter
    {
        public double ColumnWidth { get; set; }

        public bool Middle { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var column = (int)value;

            return column * ColumnWidth + (Middle ? (ColumnWidth / 2) : 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}