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
                LayoutDirection = LayoutDirection.TopToBottom,
            });
            Surface.SizeToFit();
        }

        public DiagramSurface Surface
        {
            get { return ds; }
        }
    }
}
