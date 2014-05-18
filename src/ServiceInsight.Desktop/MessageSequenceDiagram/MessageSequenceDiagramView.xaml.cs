using System.Windows.Input;
using Mindscape.WpfDiagramming;
using Mindscape.WpfDiagramming.FlowDiagrams;
using Mindscape.WpfDiagramming.Foundation;

namespace NServiceBus.Profiler.Desktop.MessageSequenceDiagram
{
    /// <summary>
    /// Interaction logic for MessageSequenceDiagramView.xaml
    /// </summary>
    public partial class MessageSequenceDiagramView : IMessageSequenceDiagramView
    {
        public MessageSequenceDiagramView()
        {
            InitializeComponent();
        }

        public void ApplyLayout()
        {
            var gridLayoutAlgorithm = new GridLayoutAlgorithm()
                {
                    Info = new FlowDiagramLayoutAlgorithmInfo(),
                    HorizontalOffset = 20.0,
                    HorizontalSpacing = 10.0,
                    VerticalOffset = 20.0,
                    VerticalSpacing = 10.0
                };

            var treeLayoutAlgorithm = new TreeLayoutAlgorithm
                {
                    Info = new FlowDiagramLayoutAlgorithmInfo(),
                    LayoutDirection = LayoutDirection.TopToBottom,
                };

            Surface.ApplyLayoutAlgorithm(new GridLayoutAlgorithm()
            {
                Info = new FlowDiagramLayoutAlgorithmInfo(),
                HorizontalOffset = 20.0,
                HorizontalSpacing = 10.0,
                VerticalOffset = 20.0,
                VerticalSpacing = 10.0
            });
        }

        public DiagramSurface Surface
        {
            get { return ds; }
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
