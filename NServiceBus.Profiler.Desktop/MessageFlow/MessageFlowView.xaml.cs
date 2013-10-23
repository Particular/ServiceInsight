using Mindscape.WpfDiagramming;
using Mindscape.WpfDiagramming.FlowDiagrams;
using Mindscape.WpfDiagramming.Foundation;

namespace NServiceBus.Profiler.Desktop.MessageFlow
{
    /// <summary>
    /// Interaction logic for MessageFlowView.xaml
    /// </summary>
    public partial class MessageFlowView : IMessageFlowView
    {
        public MessageFlowView()
        {
            InitializeComponent();
        }

        public void SizeToFit()
        {
            Surface.SizeToFit();
        }

        public void ApplyLayout()
        {
            Surface.ApplyLayoutAlgorithm(new TreeLayoutAlgorithm
            {
                Info = new FlowDiagramLayoutAlgorithmInfo(),
                LayoutDirection = LayoutDirection.TopToBottom,
            });
        }

        public DiagramSurface Surface
        {
            get { return ds; }
        }

        public void UpdateNode(IDiagramNode node)
        {
            Surface.Formatter.Layout.SetNodeBounds(node, node.Bounds);
        }

        public void UpdateConnections()
        {
            Surface.UpdateLayout();
        }
    }
}
