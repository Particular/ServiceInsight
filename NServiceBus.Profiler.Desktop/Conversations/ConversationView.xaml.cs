using WPFExtensions.Controls;

namespace NServiceBus.Profiler.Desktop.Conversations
{
    /// <summary>
    /// Interaction logic for ConversationView.xaml
    /// </summary>
    public partial class ConversationView : IConversationView
    {
        public ConversationView()
        {
            InitializeComponent();
        }

        public void ZoomToDefault()
        {
            zoomBox.Mode = ZoomControlModes.Original;
        }

        public void ZoomToFill()
        {
            zoomBox.Mode = ZoomControlModes.Fill;
        }

        public void Clear()
        {
            graphLayout.Children.Clear();
        }
    }
}
