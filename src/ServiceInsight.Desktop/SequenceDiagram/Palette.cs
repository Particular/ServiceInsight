namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System.Windows;
    using System.Windows.Media;

    public class Palette
    {
        public static readonly DependencyProperty Brush1Property = DependencyProperty.RegisterAttached(
            "Brush1", typeof(Brush), typeof(Palette), new PropertyMetadata(default(Brush)));

        public static void SetBrush1(DependencyObject element, Brush value)
        {
            element.SetValue(Brush1Property, value);
        }

        public static Brush GetBrush1(DependencyObject element)
        {
            return (Brush)element.GetValue(Brush1Property);
        }

        public static readonly DependencyProperty Brush2Property = DependencyProperty.RegisterAttached(
            "Brush2", typeof(Brush), typeof(Palette), new PropertyMetadata(default(Brush)));

        public static void SetBrush2(DependencyObject element, Brush value)
        {
            element.SetValue(Brush2Property, value);
        }

        public static Brush GetBrush2(DependencyObject element)
        {
            return (Brush)element.GetValue(Brush2Property);
        }

        public static readonly DependencyProperty Brush3Property = DependencyProperty.RegisterAttached(
            "Brush3", typeof(Brush), typeof(Palette), new PropertyMetadata(default(Brush)));

        public static void SetBrush3(DependencyObject element, Brush value)
        {
            element.SetValue(Brush3Property, value);
        }

        public static Brush GetBrush3(DependencyObject element)
        {
            return (Brush)element.GetValue(Brush3Property);
        }
    }
}