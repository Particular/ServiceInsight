using Mindscape.WpfDiagramming;
using Mindscape.WpfDiagramming.FlowDiagrams;
using Mindscape.WpfDiagramming.Foundation;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace NServiceBus.Profiler.Desktop.MessageFlow
{
    /// <summary>
    /// Interaction logic for MessageFlowView.xaml
    /// </summary>
    public partial class MessageFlowView : IMessageFlowView
    {
        public MessageFlowView()
        {
            ShowMessage = (s, e) => { };
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

        private void MessageRectangle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var node = ((System.Windows.FrameworkElement)sender).DataContext as MessageNode;
            ShowMessage(sender, new SearchMessageEventArgs(node));
        }

        public event System.EventHandler<SearchMessageEventArgs> ShowMessage;
    }
}
