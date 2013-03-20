using System;
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

        private void OnLayoutFinished(object sender, EventArgs e)
        {
            if (Model != null)
            {
                Model.GraphLayoutUpdated();
            }
        }

        private IConversationViewModel Model
        {
            get { return DataContext as IConversationViewModel; }
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
