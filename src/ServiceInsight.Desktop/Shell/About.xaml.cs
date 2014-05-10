namespace Particular.ServiceInsight.Desktop.Shell
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class AboutView
    {
        public AboutView()
        {
            InitializeComponent();
        }

        public static Window AsSplashScreen()
        {
            var vm = AboutViewModel.AsSplashScreenModel();
            var view = new AboutView {DataContext = vm};

            return view;
        }

        private AboutViewModel Model 
        {
            get { return (AboutViewModel) DataContext; }
        }

        private void OnLinkClicked(object sender, RoutedEventArgs e)
        {
            Model.NavigateToSite();
        }
    }
}
