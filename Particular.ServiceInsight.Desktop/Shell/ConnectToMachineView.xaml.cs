using System.Windows;

namespace Particular.ServiceInsight.Desktop.Shell
{
    /// <summary>
    /// Interaction logic for ConnectToMachineView.xaml
    /// </summary>
    public partial class ConnectToMachineView
    {
        public ConnectToMachineView()
        {
            InitializeComponent();
        }

        private void SelectText(object sender, RoutedEventArgs e)
        {
            name.SelectAll();
        }
    }
}
