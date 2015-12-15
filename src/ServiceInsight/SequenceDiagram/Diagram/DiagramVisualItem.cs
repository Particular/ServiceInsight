namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class DiagramVisualItem : ListBoxItem
    {
        public static DependencyProperty XProperty = DependencyProperty.Register("X", typeof(double), typeof(DiagramVisualItem), new PropertyMetadata());
        public static DependencyProperty YProperty = DependencyProperty.Register("Y", typeof(double), typeof(DiagramVisualItem), new PropertyMetadata());
        public static DependencyProperty ZIndexProperty = DependencyProperty.Register("ZIndex", typeof(int), typeof(DiagramVisualItem), new PropertyMetadata());
        public static DependencyProperty HilightProperty = DependencyProperty.Register("Hilight", typeof(bool), typeof(DiagramVisualItem), new PropertyMetadata(HilightChanged));

        public event DependencyPropertyChangedEventHandler HilightChangedEvent;

        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        public int ZIndex
        {
            get { return (int)GetValue(ZIndexProperty); }
            set { SetValue(ZIndexProperty, value); }
        }

        public bool Hilight
        {
            get { return (bool)GetValue(HilightProperty); }
            set { SetValue(HilightProperty, value); }
        }

        public void InternalSetHilight(bool value)
        {
            // Sets the value without destroying triggers or bindings.
            SetCurrentValue(HilightProperty, value);
        }

        static void HilightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DiagramVisualItem)d).OnHilightChanged(e);
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            //NOTE: To prevent selection on right click
        }

        protected virtual void OnHilightChanged(DependencyPropertyChangedEventArgs e)
        {
            HilightChangedEvent?.Invoke(this, new DependencyPropertyChangedEventArgs(HilightProperty, e.OldValue, e.NewValue));
        }
    }
}