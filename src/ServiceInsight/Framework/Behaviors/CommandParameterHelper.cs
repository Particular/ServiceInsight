namespace ServiceInsight.Framework.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    public class CommandParameterHelper
    {
        public static DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(CommandParameterHelper),
            new PropertyMetadata(OnCommandParameterChanged));

        static void OnCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var menu = d as MenuItem;
            if (menu != null)
            {
                SwapCommand(menu, e.NewValue);
            }

            var btn = d as ButtonBase;
            if (btn != null)
            {
                SwapCommand(btn, e.NewValue);
            }
        }

        static void SwapCommand(dynamic commandHolder, object parameter)
        {
            commandHolder.CommandParameter = parameter;
            var temp = commandHolder.Command;
            // Have to set it to null first or CanExecute won't be called.
            commandHolder.Command = null;
            commandHolder.Command = temp;

        }

        public static object GetCommandParameter(UIElement target)
        {
            return target.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(UIElement target, object value)
        {
            target.SetValue(CommandParameterProperty, value);
        }
    }
}