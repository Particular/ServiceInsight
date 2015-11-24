
namespace Particular.ServiceInsight.Desktop.Controls
{
    using System.Windows.Controls;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Controls.Primitives;
    using Particular.ServiceInsight.Desktop.Framework.Behaviors;

    /// <summary>
	/// Represents a combination of a standard button on the left and a drop-down button on the right.
	/// </summary>
	[TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_Button", Type = typeof(Button))]
    public class SplitButton : MenuItem
    {
        private Button splitButtonHeaderSite;

        /// <summary>
        /// Identifies the CornerRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty;

        public static DependencyProperty ButtonClickProperty = DependencyProperty.Register("ButtonClick", typeof(ICommand), typeof(SplitButton));
        public static DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(SplitButton));

	    static SplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));

            CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner(typeof(SplitButton));

            IsSubmenuOpenProperty.OverrideMetadata(typeof(SplitButton),
                new FrameworkPropertyMetadata(
                    BooleanBoxes.FalseBox,
                    OnIsSubmenuOpenChanged,
                    CoerceIsSubmenuOpen));

            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(KeyboardNavigationMode.Local));
            KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));

            EventManager.RegisterClassHandler(typeof(SplitButton), MenuItem.ClickEvent, new RoutedEventHandler(OnMenuItemClick));
            EventManager.RegisterClassHandler(typeof(SplitButton), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButtonDown), true);
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public ICommand ButtonClick
        {
            get { return (ICommand)GetValue(ButtonClickProperty); }
            set { SetValue(ButtonClickProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool) GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

	    public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            splitButtonHeaderSite = GetTemplateChild("PART_Button") as Button;
            if (splitButtonHeaderSite != null)
            {
                splitButtonHeaderSite.CommandParameter = CommandParameterHelper.GetCommandParameter(this);
                splitButtonHeaderSite.Command = ButtonClick;
            }
        }

        private static void OnIsSubmenuOpenChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var splitButton = (SplitButton)sender;
            if ((bool)e.NewValue)
            {
                if (Mouse.Captured != splitButton)
                {
                    Mouse.Capture(splitButton, CaptureMode.SubTree);
                }
            }
            else
            {
                if (Mouse.Captured == splitButton)
                {
                    Mouse.Capture(null);
                }

                if (splitButton.IsKeyboardFocused)
                {
                    splitButton.Focus();
                }
            }
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            CloseSubmenu();
            base.OnPreviewMouseWheel(e);
        }

        /// <summary>
        /// Set the IsSubmenuOpen property value at the right time.
        /// </summary>
        private static object CoerceIsSubmenuOpen(DependencyObject element, object value)
        {
            var splitButton = (SplitButton)element;
            if ((bool)value)
            {
                if (!splitButton.IsLoaded)
                {
                    splitButton.Loaded += (sender, e) => splitButton.CoerceValue(IsSubmenuOpenProperty);

                    return BooleanBoxes.FalseBox;
                }
            }

            return (bool)value && splitButton.HasItems;
        }

        private static void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            //SplitButton splitButton = sender as SplitButton;
            MenuItem menuItem = e.OriginalSource as MenuItem;

            // To make the ButtonClickEvent get fired as we expected, you should mark the ClickEvent 
            // as handled to prevent the event from poping up to the button portion of the SplitButton.
            if (menuItem != null && !typeof(MenuItem).IsAssignableFrom(menuItem.Parent.GetType()))
            {
                e.Handled = true;
            }
        }

        private static void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
	        if (e.LeftButton == MouseButtonState.Pressed)
	        {
		        OnMouseLeftButtonClicked(sender, e);
	        }

	        if (e.RightButton == MouseButtonState.Pressed)
	        {
		        OnMouseRightButtonClicked(sender, e);
	        }
            
        }

	    static void OnMouseRightButtonClicked(object sender, MouseButtonEventArgs e)
	    {
			var splitButton = (SplitButton)sender;
			splitButton.OpenSubmenu();
		}

	    static void OnMouseLeftButtonClicked(object sender, MouseButtonEventArgs e)
	    {
			var splitButton = (SplitButton)sender;
			if (e.OriginalSource == splitButton || e.Source == splitButton)
			{
				splitButton.CloseSubmenu();
				return;
			}

			if (!splitButton.IsKeyboardFocusWithin)
			{
				splitButton.Focus();
				return;
			}

			if (e.Source is MenuItem)
			{
				var menuItem = (MenuItem)e.Source;
				if (menuItem != null)
				{
					if (!menuItem.HasItems)
					{
						splitButton.CloseSubmenu();
						menuItem.Command.Execute(menuItem.CommandParameter);
					}
				}
			}
		}

	    void OpenSubmenu()
		{
			if (!IsSubmenuOpen)
			{
				IsSubmenuOpen = true;
			}
		}

		void CloseSubmenu()
        {
            if (IsSubmenuOpen)
            {
                ClearValue(IsSubmenuOpenProperty);
                if (IsSubmenuOpen)
                {
                    IsSubmenuOpen = false;
                }
            }
        }
    }

    internal static class BooleanBoxes
    {
        public static readonly object TrueBox = true;
        public static readonly object FalseBox = false;

        public static object Box(bool value)
        {
            if (value)
            {
                return TrueBox;
            }
            else
            {
                return FalseBox;
            }
        }
    }
}