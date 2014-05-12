namespace Particular.ServiceInsight.Desktop.ValueConverters
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Data;
    using Mindscape.WpfDiagramming.Foundation;

    public class StartSegmentValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var segments = (ObservableCollection<DiagramConnectionSegment>)value;

            if (parameter == null)
            {
                return segments.First().StartPoint;
            }
            if (parameter.ToString() == "X")
            {
                return segments.First().StartPoint.X;
            }
            return segments.First().StartPoint.Y;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}