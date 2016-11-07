namespace ServiceInsight.Shell
{
    using System.Windows;

    public partial class AboutView
    {
        public AboutView()
        {
            InitializeComponent();
        }

        public static Window AsSplashScreen()
        {
            var vm = AboutViewModel.AsSplashScreenModel();
            var view = new AboutView { DataContext = vm, WindowStyle = WindowStyle.None };

            return view;
        }
    }
}