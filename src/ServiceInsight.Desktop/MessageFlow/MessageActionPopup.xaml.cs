namespace Particular.ServiceInsight.Desktop.MessageFlow
{
    using System;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MessageActionPopup.xaml
    /// </summary>
    public partial class MessageActionPopup
    {
        public MessageActionPopup()
        {
            InitializeComponent();
        }

        public event Action RequestToClose = () => { };

        private void OnMenuClicked(object sender, RoutedEventArgs e)
        {
            RequestToClose();
        }
    }
}
