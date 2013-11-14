using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using Mindscape.WpfDiagramming.Foundation;

namespace NServiceBus.Profiler.Desktop.ValueConverters
{
    public class EndSegmentValueConverter : IValueConverter
    {
        private const double arrowSize = 20;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var segments = (ObservableCollection<DiagramConnectionSegment>)value;

            var angle = AngleFromSegments(segments);

            if (parameter == null)
            {
                return segments.Last().EndPoint;
            }
            if (parameter.ToString() == "X")
            {
                return segments.Last().EndPoint.X + (arrowSize * Math.Sin(angle));
            }
            else
            {
                return segments.Last().EndPoint.Y + (arrowSize * Math.Cos(angle));
            }
        }

        private static double AngleFromSegments(ObservableCollection<DiagramConnectionSegment> segments)
        {
            return Math.Atan2(segments.Last().EndPoint.X - segments.First().EndPoint.X, segments.Last().EndPoint.Y - segments.First().EndPoint.Y);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}