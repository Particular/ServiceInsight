namespace ServiceInsight.MessageFlow
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Mindscape.WpfDiagramming;
    using Mindscape.WpfDiagramming.Foundation;

    public partial class MessageFlowView
    {
        public event EventHandler<SearchMessageEventArgs> ShowMessage;

        public MessageFlowView()
        {
            ShowMessage = (s, e) => { };
            InitializeComponent();

            ds.AddHandler(DiagramNodeElement.BoundsChangeRequestedEvent, new EventHandler<BoundsChangeRequestedEventArgs>(OnBoundsChangeRequested));
        }

        public void ApplyLayout()
        {
            if (!IsVisible) return;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                Surface.ApplyLayoutAlgorithm(new TreeLayoutAlgorithm());
                Surface.SizeToFit();

                if (ds.Diagram.Nodes.Count <= 5)
                {
                    Surface.Zoom = 1;
                }
            }), DispatcherPriority.Loaded);
        }

        public DiagramSurface Surface
        {
            get { return ds; }
        }

        void MessageRectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var node = ((FrameworkElement)sender).DataContext as MessageNode;
            ShowMessage(sender, new SearchMessageEventArgs(node));
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

        private void OnBoundsChangeRequested(object sender, BoundsChangeRequestedEventArgs e)
        {
            var node = e.DiagramNodeElement.Node;
            node.Bounds = new Rect(node.Bounds.X, node.Bounds.Y, e.Width, node.Bounds.Height);
        }

        void OnDiagramVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ApplyLayout();
        }

        void OnDiagramSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ApplyLayout();
        }
    }
}