using System.Windows;

namespace NServiceBus.Profiler.Desktop.Shell
{
    /// <summary>
    /// Interaction logic for ManagementConnectionView.xaml
    /// </summary>
    public partial class ManagementConnectionView
    {
        public ManagementConnectionView()
        {
            InitializeComponent();
        }

        private void SelectText(object sender, RoutedEventArgs e)
        {
            name.SelectAll();
        }
    }
}
