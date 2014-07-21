namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System.Windows;
    using System.Windows.Media;

    public class SagaPalette
    {
        public static readonly DependencyProperty LineProperty = DependencyProperty.RegisterAttached(
            "Line", typeof(Brush), typeof(SagaPalette), new PropertyMetadata(default(Brush)));

        public static void SetLine(DependencyObject element, Brush value)
        {
            element.SetValue(LineProperty, value);
        }

        public static Brush GetLine(DependencyObject element)
        {
            return (Brush)element.GetValue(LineProperty);
        }

        public static readonly DependencyProperty EndpointBoxProperty = DependencyProperty.RegisterAttached(
            "EndpointBox", typeof(Brush), typeof(SagaPalette), new PropertyMetadata(default(Brush)));

        public static void SetEndpointBox(DependencyObject element, Brush value)
        {
            element.SetValue(EndpointBoxProperty, value);
        }

        public static Brush GetEndpointBox(DependencyObject element)
        {
            return (Brush)element.GetValue(EndpointBoxProperty);
        }

        public static readonly DependencyProperty MessageBackgroundProperty = DependencyProperty.RegisterAttached(
            "MessageBackground", typeof(Brush), typeof(SagaPalette), new PropertyMetadata(default(Brush)));

        public static void SetMessageBackground(DependencyObject element, Brush value)
        {
            element.SetValue(MessageBackgroundProperty, value);
        }

        public static Brush GetMessageBackground(DependencyObject element)
        {
            return (Brush)element.GetValue(MessageBackgroundProperty);
        }

        public static readonly DependencyProperty MessageForegroundProperty = DependencyProperty.RegisterAttached(
            "MessageForeground", typeof(Brush), typeof(SagaPalette), new PropertyMetadata(default(Brush)));

        public static void SetMessageForeground(DependencyObject element, Brush value)
        {
            element.SetValue(MessageForegroundProperty, value);
        }

        public static Brush GetMessageForeground(DependencyObject element)
        {
            return (Brush)element.GetValue(MessageForegroundProperty);
        }

        public static readonly DependencyProperty SagaBackgroundProperty = DependencyProperty.RegisterAttached(
            "SagaBackground", typeof(Brush), typeof(SagaPalette), new PropertyMetadata(default(Brush)));

        public static void SetSagaBackground(DependencyObject element, Brush value)
        {
            element.SetValue(SagaBackgroundProperty, value);
        }

        public static Brush GetSagaBackground(DependencyObject element)
        {
            return (Brush)element.GetValue(SagaBackgroundProperty);
        }

        public static readonly DependencyProperty SagaForegroundProperty = DependencyProperty.RegisterAttached(
            "SagaForeground", typeof(Brush), typeof(SagaPalette), new PropertyMetadata(default(Brush)));

        public static void SetSagaForeground(DependencyObject element, Brush value)
        {
            element.SetValue(SagaForegroundProperty, value);
        }

        public static Brush GetSagaForeground(DependencyObject element)
        {
            return (Brush)element.GetValue(SagaForegroundProperty);
        }
    }
}