using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using Mindscape.WpfDiagramming.Foundation;

namespace NServiceBus.Profiler.Desktop.ValueConverters
{
    public class StartSegmentValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var segments = (ObservableCollection<DiagramConnectionSegment>)value;

            if (parameter == null)
            {
                return segments.First().StartPoint;
            }
            else if (parameter.ToString() == "X")
            {
                return segments.First().StartPoint.X;
            }
            else
            {
                return segments.First().StartPoint.Y;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}