namespace Particular.ServiceInsight.Desktop.MessageViewers
{
    using System.Windows;
    using DevExpress.Xpf.Core;

    /// <summary>
    /// Interaction logic for MessageBodyView.xaml
    /// </summary>
    public partial class MessageBodyView
    {
        public MessageBodyView()
        {
            InitializeComponent();
        }

        void OnSelectedTabChanged(object sender, ValueChangedEventArgs<FrameworkElement> e)
        {
            var newElement = e.NewValue;
            var oldElement = e.OldValue;
        }
    }
}
