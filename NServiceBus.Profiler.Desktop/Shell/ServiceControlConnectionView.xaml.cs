using System.Windows;

namespace NServiceBus.Profiler.Desktop.Shell
{
    /// <summary>
    /// Interaction logic for ServiceControlConnectionView.xaml
    /// </summary>
    public partial class ServiceControlConnectionView
    {
        public ServiceControlConnectionView()
        {
            InitializeComponent();
        }

        private void SelectText(object sender, RoutedEventArgs e)
        {
            name.SelectAll();
        }
    }
}
