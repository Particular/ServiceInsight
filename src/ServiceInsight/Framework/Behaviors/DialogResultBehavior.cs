namespace ServiceInsight.Framework.Behaviors
{
    using System.Windows;

    public class DialogResultBehavior
    {
        public static readonly DependencyProperty ResultProperty = DependencyProperty.RegisterAttached("Result", typeof(bool?), typeof(DialogResultBehavior), new PropertyMetadata(DialogResultChanged));

        private static void DialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (window != null && window.IsVisible)
            {
                window.DialogResult = e.NewValue as bool?;
            }
        }

        public static void SetResult(Window target, bool? value)
        {
            target.SetValue(ResultProperty, value);
        }
    }
}