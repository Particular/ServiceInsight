namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System.Windows;
    using System.Windows.Controls;

    public class DiagramVisualItem : ListBoxItem
    {
        public static DependencyProperty XProperty = DependencyProperty.Register("X", typeof(double), typeof(DiagramVisualItem), new PropertyMetadata());
        public static DependencyProperty YProperty = DependencyProperty.Register("Y", typeof(double), typeof(DiagramVisualItem), new PropertyMetadata());

        public double X
        {
            get { return (double) GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }
    }
}