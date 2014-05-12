namespace Particular.ServiceInsight.Desktop.Shell
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for ConnectToMachineView.xaml
    /// </summary>
    public partial class ConnectToMachineView
    {
        public ConnectToMachineView()
        {
            InitializeComponent();
        }

        void SelectText(object sender, RoutedEventArgs e)
        {
            name.SelectAll();
        }
    }
}
