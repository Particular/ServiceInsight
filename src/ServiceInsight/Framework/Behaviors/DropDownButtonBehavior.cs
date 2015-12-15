namespace ServiceInsight.Framework.Behaviors
{
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Interactivity;

    public class DropDownButtonBehavior : Behavior<ButtonBase>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(AssociatedObject_Click), true);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(AssociatedObject_Click));
        }

        void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            var source = sender as ButtonBase;
            if (source != null && source.ContextMenu != null)
            {
                // If there is a drop-down assigned to this button, then position and display it
                source.ContextMenu.PlacementTarget = source;
                source.ContextMenu.Placement = PlacementMode.Bottom;
                source.ContextMenu.IsOpen = true;
            }
        }
    }
}