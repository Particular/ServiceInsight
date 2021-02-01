namespace ServiceInsight.Shell
{
    using System.Windows;
    using System.Windows.Controls.Primitives;
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

        void OnCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            TryClosePopup();
        }

        void TryClosePopup()
        {
            var popup = this.TryFindParent<Popup>();
            if (popup != null)
            {
                popup.IsOpen = false;
            }
        }
    }
}