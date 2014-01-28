using Mindscape.WpfDiagramming;
using Mindscape.WpfDiagramming.FlowDiagrams;
using Mindscape.WpfDiagramming.Foundation;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

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

        private void Root_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (e.Key == Key.OemPlus || e.Key == Key.Add)
                {
                    Surface.Zoom += 0.1;
                }
                else if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
                {
                    Surface.Zoom -= 0.1;
                }
                else if (e.Key == Key.D0 || e.Key == Key.NumPad0) 
                {
                    Surface.Zoom = 1;
                }
                else if (e.Key == Key.D1 || e.Key == Key.NumPad1)
                {
                    Surface.SizeToFit();
                }
            }
        }
    }
}
