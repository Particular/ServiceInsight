namespace ServiceInsight.SequenceDiagram.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using ServiceInsight.SequenceDiagram.Diagram;

    public class CalculateArrowLineWidth:IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Direction direction;
            Enum.TryParse(values[0].ToString(), out direction);

            if (direction == Direction.Right)
            {
                return 0;
            }

            return -(double)values[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
