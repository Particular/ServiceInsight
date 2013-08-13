using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Particular.ServiceInsight.Desktop.Models;

namespace Particular.ServiceInsight.Desktop.ValueConverters
{
    public class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var app = Application.Current;

            if (value is MessageStatus)
            {
                var status = (MessageStatus) value;
                switch (status)
                {
                    case MessageStatus.Failed:
                        return app.TryFindResource("DiagramFailedNodeBackground") as SolidColorBrush;
                    case MessageStatus.RepeatedFailures:
                        return app.TryFindResource("DiagramRepeatedFailedNodeBackground") as SolidColorBrush;
                    case MessageStatus.Successful:
                        return app.TryFindResource("DiagramSuccessNodeBackground") as SolidColorBrush;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}