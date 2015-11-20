using System.Windows.Controls;

namespace ServiceInsight.SequenceDiagram
{
    using System.Windows.Input;

    public partial class SequenceDiagramView
    {
        public SequenceDiagramView()
        {
            InitializeComponent();

            ScrollViewer_Body.ScrollChanged += ScrollViewer_Body_ScrollChanged;
            ScrollViewer_Body.PreviewMouseWheel += ScrollViewer_Body_MouseWheel;
        }

        void ScrollViewer_Body_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer_Body.ScrollToVerticalOffset(ScrollViewer_Body.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        void ScrollViewer_Body_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer_Header.Width = e.ViewportWidth;
            ScrollViewer_Header.ScrollToHorizontalOffset(e.HorizontalOffset);
        }
    }
}