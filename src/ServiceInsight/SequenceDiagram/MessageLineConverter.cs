namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;

    public abstract class BaseMessageLineConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var endpoint = values[0] as EndpointInfo;
            var message = values[1] as MessageInfo;

            if (endpoint == null || message == null)
                return Default;

            return Convert(message, endpoint);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new Exception("Binding should be one way only.");
        }

        protected abstract object Default { get; }

        protected abstract object Convert(MessageInfo message, EndpointInfo endpoint);
    }

    public class MessageLineConverter : BaseMessageLineConverter
    {
        protected override object Default
        {
            get { return Visibility.Hidden; }
        }

        protected override object Convert(MessageInfo message, EndpointInfo endpoint)
        {
            return message.EndpointNeedsMessageLine(endpoint) ? Visibility.Visible : Visibility.Hidden;
        }
    }

    public class MessageLineIsPublishedConverter : BaseMessageLineConverter
    {
        protected override object Default
        {
            get { return new DoubleCollection(); }
        }

        protected override object Convert(MessageInfo message, EndpointInfo endpoint)
        {
            return message.EndpointMessageLineIsPublished(endpoint) ? new DoubleCollection(new [] { 1.5 }) : new DoubleCollection();
        }
    }
}