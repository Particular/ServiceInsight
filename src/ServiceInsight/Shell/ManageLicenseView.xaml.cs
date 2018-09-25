namespace ServiceInsight.Shell
{
    using System.Windows;
    using System.Windows.Controls;
    using ServiceInsight.ExtensionMethods;

    /// <summary>
    /// Interaction logic for ManageLicenseView.xaml
    /// </summary>
    public partial class ManageLicenseView
    {
        public ManageLicenseView()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var closeButton = this.TryFindChild<Button>("PART_CloseButton");

            if (closeButton != null)
            {
                closeButton.Click += OnCloseButtonClicked;
            }
        }

        private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = null;
            Close();
        }
    }
}
