namespace Particular.ServiceInsight.Desktop.Framework.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;

    public class CommandParameterHelper
    {
        public static DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(CommandParameterHelper),
            new PropertyMetadata(OnCommandParameterChanged));

        static void OnCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = d as MenuItem;
            if (target == null)
                return;

            target.CommandParameter = e.NewValue;
            var temp = target.Command;
            // Have to set it to null first or CanExecute won't be called.
            target.Command = null;
            target.Command = temp;
        }

        public static object GetCommandParameter(MenuItem target)
        {
            return target.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(MenuItem target, object value)
        {
            target.SetValue(CommandParameterProperty, value);
        }
    }
}