namespace ServiceInsight.Shell
{
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using ServiceInsight.ExtensionMethods;

    /// <summary>
    /// Interaction logic for LicensePopupControl.xaml
    /// </summary>
    public partial class LicensePopupView
    {
        public LicensePopupView()
        {
            InitializeComponent();
        }

        private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            TryClosePopup();
        }

        private void TryClosePopup()
        {
            var popup = this.TryFindParent<Popup>();
            if (popup != null)
            {
                popup.IsOpen = false;
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TryClosePopup();
        }
    }
}