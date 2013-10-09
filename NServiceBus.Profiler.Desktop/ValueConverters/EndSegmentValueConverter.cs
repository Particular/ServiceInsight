using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using Mindscape.WpfDiagramming.Foundation;

namespace NServiceBus.Profiler.Desktop.ValueConverters
{
    public class EndSegmentValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var segments = (ObservableCollection<DiagramConnectionSegment>)value;

            if (parameter == null)
            {
                return segments.Last().EndPoint;
            }
            if (parameter.ToString() == "X")
            {
                return segments.Last().EndPoint.X;
            }
            else
            {
                return segments.Last().EndPoint.Y;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}