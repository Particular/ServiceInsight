namespace Particular.ServiceInsight.Desktop.ValueConverters
{
	using System;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Windows.Data;
	using Mindscape.WpfDiagramming.Foundation;

    public class ArrowEndSegmentValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var segments = (ObservableCollection<DiagramConnectionSegment>)value;

            object toReturn = null;
            if (parameter == null)
            {
                toReturn = segments.Last().EndPoint;
            }
            else if (parameter.ToString() == "X")
            {
                toReturn = segments.Last().EndPoint.X;
            }
            else
            {
                toReturn = segments.Last().EndPoint.Y;
            }
            return toReturn;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}