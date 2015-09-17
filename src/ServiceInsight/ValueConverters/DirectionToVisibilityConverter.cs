namespace Particular.ServiceInsight.Desktop.ValueConverters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using global::ServiceInsight.SequenceDiagram.Diagram;

    public class DirectionToVisibilityConverter : IValueConverter
    {
        public Direction Direction { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Direction direction;
            Enum.TryParse(value.ToString(), out direction);
            return direction == Direction ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ArrowTypeToBoolConverter : IValueConverter
    {
        public ArrowType Type { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ArrowType arrowType;
            Enum.TryParse(value.ToString(), out arrowType);
            return arrowType == Type;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}