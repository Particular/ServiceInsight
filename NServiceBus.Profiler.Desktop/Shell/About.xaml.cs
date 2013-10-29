using System.Windows;
using System.Windows.Input;

namespace NServiceBus.Profiler.Desktop.Shell
{
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

        private void OnImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            Model.NavigateToSite();
        }
    }
}
