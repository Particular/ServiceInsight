using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Bars;
using NServiceBus.Profiler.Desktop.Shell.Menu;

namespace NServiceBus.Profiler.Desktop.Core.UI
{
    public class NotifyMenuOpeningBehavior : DependencyObject
    {
        public static readonly DependencyProperty NotifyMenuOpeningProperty = DependencyProperty.RegisterAttached("NotifyMenuOpening", typeof(bool), typeof(NotifyMenuOpeningBehavior), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.AffectsRender, OnNotifyMenuOpeningChanged));

        public static void SetNotifyMenuOpening(PopupMenu element, bool value)
        {
            element.SetValue(NotifyMenuOpeningProperty, value);
        }

        public static bool GetNotifyMenuOpening(PopupMenu element)
        {
            return (bool)element.GetValue(NotifyMenuOpeningProperty);
        }

        private static void OnNotifyMenuOpeningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var menu = (PopupMenu) d;
            var newValue = (bool) e.NewValue;
            var oldValue = (bool) e.OldValue;

            if (oldValue && !newValue)
            {
                menu.Opening -= OnMenuOpening;
            }
            else if (newValue)
            {
                menu.Opening += OnMenuOpening;
            }
        }

        private static void OnMenuOpening(object sender, CancelEventArgs e)
        {
            var menu = (PopupMenu)sender;
            var target = menu.PlacementTarget as FrameworkElement;

            if (target != null)
            {
                var model = target.DataContext as IHaveContextMenu;
                if (model != null)
                {
                    model.OnContextMenuOpening();
                }
            }
        }
    }
}