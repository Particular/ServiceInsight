namespace ServiceInsight.SequenceDiagram.Converter
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using ServiceInsight.SequenceDiagram.Diagram;

    public class ArrowTypeToVisibilityConverter : IValueConverter
    {
        public ArrowType Type { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ArrowType arrowType;
            Enum.TryParse(value.ToString(), out arrowType);
            return arrowType == Type ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}