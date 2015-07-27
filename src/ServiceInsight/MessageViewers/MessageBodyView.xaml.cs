namespace Particular.ServiceInsight.Desktop.MessageViewers
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Documents;

    public partial class MessageBodyView
    {
        public MessageBodyView()
        {
            InitializeComponent();
        }

        void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var link = (Hyperlink) sender;

            var url = link.NavigateUri.AbsoluteUri;

            Process.Start(new ProcessStartInfo(url));
        }
    }
}
