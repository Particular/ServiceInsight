namespace Particular.ServiceInsight.Desktop.Core.UI.ScreenManager
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for Dialog.xaml
    /// </summary>
    public partial class Dialog
    {
        public static readonly DependencyProperty IsModalProperty;
       
        static Dialog()
        {
            IsModalProperty = DependencyProperty.RegisterAttached("IsModalProperty", typeof(bool), typeof(Dialog), new PropertyMetadata(false));
        }

        public Dialog()
            : this(ActiveModalWindow)
        {
        }

        public Dialog(Window parent)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ShowInTaskbar = false;
            SnapsToDevicePixels = true;

            if (parent != null)
            {
                Owner = parent;
                FlowDirection = parent.FlowDirection;
            }
        }

        public static Window ActiveModalWindow
        {
            get
            {
                var window = FindFirstModalDialog(Application.Current.MainWindow);
                return window ?? Application.Current.MainWindow;
            }
        }

        public new bool? ShowDialog()
        {
            bool? result;

            try
            {
                SetValue(IsModalProperty, true);
                result = base.ShowDialog();
            }
            finally
            {
                SetValue(IsModalProperty, false);
            }

            return result;
        }

        private static Window FindFirstModalDialog(Window ownerWindow)
        {
            if (ownerWindow != null)
            {
                if (ownerWindow.OwnedWindows.Count != 0)
                {
                    foreach (Window w in ownerWindow.OwnedWindows)
                    {
                        var window = FindFirstModalDialog(w);
                        if (window != null)
                            return window;
                    }
                }

                if ((bool)ownerWindow.GetValue(IsModalProperty) && ownerWindow.IsEnabled && ownerWindow.IsVisible && ownerWindow.IsActive)
                    return ownerWindow;
            }

            return null;
        }

        public static bool GetIsModalProperty(Window window)
        {
            return (bool)window.GetValue(IsModalProperty);
        }

        public static void SetIsModalProperty(Window window, bool value)
        {
            window.SetValue(IsModalProperty, value);
        }
    }
}
