namespace ServiceInsight.Shell
{
    using System.Windows;

    public partial class ServiceControlConnectionView
    {
        public ServiceControlConnectionView()
        {
            InitializeComponent();
        }

        void SelectText(object sender, RoutedEventArgs e)
        {
            name.SelectAll();
        }
    }
}
