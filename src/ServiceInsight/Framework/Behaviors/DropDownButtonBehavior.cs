namespace Particular.ServiceInsight.Desktop.Framework.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Interactivity;

    public class DropDownButtonBehavior : Behavior<Button>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AddHandler(Button.ClickEvent, new RoutedEventHandler(AssociatedObject_Click), true);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.RemoveHandler(Button.ClickEvent, new RoutedEventHandler(AssociatedObject_Click));
        }

        void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            var source = sender as Button;
            if (source != null && source.ContextMenu != null)
            {
                // If there is a drop-down assigned to this button, then position and display it
                source.ContextMenu.PlacementTarget = source;
                source.ContextMenu.Placement = PlacementMode.Bottom;
                source.ContextMenu.IsOpen = true;
            }
        }
    }

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