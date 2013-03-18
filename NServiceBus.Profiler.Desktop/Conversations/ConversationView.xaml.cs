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
            GraphLayout.LayoutUpdateFinished += OnLayoutFinished;
            GraphLayout.IsAnimationEnabled = false;
            GraphLayout.DestructionTransition = null;
            GraphLayout.AnimationLength = TimeSpan.FromMilliseconds(0);
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
