using System.Windows.Navigation;

namespace NServiceBus.Profiler.Desktop.About
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView
    {
        public AboutView()
        {
            InitializeComponent();
        }

        private void OnNavigate(object sender, RequestNavigateEventArgs e)
        {
            Model.NavigateToWebsite();
        }

        private AboutViewModel Model
        {
            get { return DataContext as AboutViewModel; }
        }
    }
}
