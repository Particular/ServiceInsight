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
