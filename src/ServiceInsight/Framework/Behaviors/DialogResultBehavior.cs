namespace ServiceInsight.Framework.Behaviors
{
    using System.Windows;

    public class DialogResultBehavior
    {
        public static readonly DependencyProperty ResultProperty = DependencyProperty.RegisterAttached("Result", typeof(bool?), typeof(DialogResultBehavior), new PropertyMetadata(OnResultChanged));
        public static readonly DependencyProperty AutoCloseProperty = DependencyProperty.RegisterAttached("AutoClose", typeof(bool), typeof(DialogResultBehavior), new PropertyMetadata(true));

        static void OnResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window && GetAutoClose(window))
            {
                if (window.IsVisible)
                {
                    var result = window.GetValue(ResultProperty) as bool?;
                    window.DialogResult = result;
                }
            }
        }

        public static void SetResult(Window target, bool? value)
        {
            target.SetValue(ResultProperty, value);
        }

        public static bool? GetResult(Window target)
        {
            return target.GetValue(ResultProperty) as bool?;
        }

        public static bool GetAutoClose(UIElement element)
        {
            return (bool)element.GetValue(AutoCloseProperty);
        }

        public static void SetAutoClose(UIElement element, bool value)
        {
            element.SetValue(AutoCloseProperty, value);
        }
    }
}