using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WPFExtensions.Controls;
using System.Linq;

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
            ZoomViewer.Mode = ZoomControlModes.Original;
        }

        public void ZoomToFill()
        {
            ZoomViewer.Mode = ZoomControlModes.Fill;
        }

        public void Clear()
        {
            GraphLayout.Children.Clear();
        }
    }
}
