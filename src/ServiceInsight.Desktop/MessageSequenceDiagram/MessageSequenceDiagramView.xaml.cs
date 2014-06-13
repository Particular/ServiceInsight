namespace Particular.ServiceInsight.Desktop.MessageSequenceDiagram
{
    using System;
    using System.Windows.Input;
    using Mindscape.WpfDiagramming;
    using Mindscape.WpfDiagramming.FlowDiagrams;
    using Mindscape.WpfDiagramming.Foundation;

    public partial class MessageSequenceDiagramView : IMessageSequenceDiagramView
    {
        public MessageSequenceDiagramView()
        {
            InitializeComponent();
        }

        public void ApplyLayout()
        {
            Surface.ApplyLayoutAlgorithm(new GridLayoutAlgorithm()
            {
                Info = new FlowDiagramLayoutAlgorithmInfo(), 
                HorizontalOffset = 0.0,
                HorizontalSpacing = 20.0,
                VerticalOffset = 0.0,
                VerticalSpacing = 20.0
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

        public void SizeToFit()
        {
            Surface.SizeToFit();
        }

        private void Root_KeyUp(object sender, KeyEventArgs e)
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
