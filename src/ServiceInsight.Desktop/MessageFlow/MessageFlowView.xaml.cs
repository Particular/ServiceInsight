namespace Particular.ServiceInsight.Desktop.MessageFlow
{
    using System;
    using System.Windows.Input;
    using Mindscape.WpfDiagramming;
    using Mindscape.WpfDiagramming.FlowDiagrams;
    using Mindscape.WpfDiagramming.Foundation;

    public partial class MessageFlowView : IMessageFlowView
    {
        public event EventHandler<SearchMessageEventArgs> ShowMessage;

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

        void MessageRectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var node = ((System.Windows.FrameworkElement)sender).DataContext as MessageNode;
                ShowMessage(sender, new SearchMessageEventArgs(node));
            }
        }

        void Root_KeyUp(object sender, KeyEventArgs e)
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
