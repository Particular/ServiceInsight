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
            var vm = new AboutViewModel();
            var view = new AboutView { DataContext = vm, WindowStyle = WindowStyle.None };

            return view;
        }
    }
}