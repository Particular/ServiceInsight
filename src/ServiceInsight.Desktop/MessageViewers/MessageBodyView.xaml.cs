using System.Windows;
using DevExpress.Xpf.Core;

namespace NServiceBus.Profiler.Desktop.MessageViewers
{
    /// <summary>
    /// Interaction logic for MessageBodyView.xaml
    /// </summary>
    public partial class MessageBodyView
    {
        public MessageBodyView()
        {
            InitializeComponent();
        }

        private void OnSelectedTabChanged(object sender, ValueChangedEventArgs<FrameworkElement> e)
        {
            var newElement = e.NewValue;
            var oldElement = e.OldValue;
        }
    }
}
