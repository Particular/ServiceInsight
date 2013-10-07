using Mindscape.WpfDiagramming;
using Mindscape.WpfDiagramming.FlowDiagrams;
using Mindscape.WpfDiagramming.Foundation;

namespace NServiceBus.Profiler.Desktop.MessageFlow
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

        private IConversationViewModel Model
        {
            get { return DataContext as IConversationViewModel; }
        }

        public void ZoomToDefault()
        {
            
        }

        public void ZoomToFill()
        {
            Surface.SizeToFit();
        }

        public void ApplyLayout()
        {
            Surface.ApplyLayoutAlgorithm(new TreeLayoutAlgorithm
            {
                Info = new FlowDiagramLayoutAlgorithmInfo(),
                LayoutDirection = LayoutDirection.LeftToRight,
            });
            Surface.SizeToFit();
        }

        public DiagramSurface Surface
        {
            get { return ds; }
        }
    }

/*
    public class MyLayoutAlgorithmInfo : ILayoutAlgorithmInfo
    {
        public bool IsIncluded(IDiagramNode node)
        {
            return node is MessageNode;
        }
    }

*/
}
