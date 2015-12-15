namespace ServiceInsight.MessageFlow
{
    using System;
    using System.Windows;

    public partial class MessageActionPopup
    {
        public MessageActionPopup()
        {
            InitializeComponent();
        }

        public event Action RequestToClose = () => { };

        void OnMenuClicked(object sender, RoutedEventArgs e)
        {
            RequestToClose();
        }
    }
}
