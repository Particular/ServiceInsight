using System;
using System.Windows;

namespace NServiceBus.Profiler.Desktop.MessageFlow
{
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
