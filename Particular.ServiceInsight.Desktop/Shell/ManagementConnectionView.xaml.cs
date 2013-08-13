using System.Windows;

namespace Particular.ServiceInsight.Desktop.Shell
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
