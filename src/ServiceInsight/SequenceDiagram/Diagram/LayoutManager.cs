namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System.Linq;
    using System.Windows.Controls;

    public class SequenceDiagramLayoutManager : ILayoutManager
    {
        public void PerformLayout(IDiagram diagram)
        {
            LayoutEndpoints(diagram);
        }

        private void LayoutEndpoints(IDiagram diagram)
        {
            var endpoints = diagram.Items.OfType<EndpointItem>().ToList();

            ListBoxItem lastItem = null;
            EndpointItem lastEndpoint = null;
            var firstItemLocation = 0d;
            var index = 0;

            foreach (var endpoint in endpoints)
            {
                var item = diagram.GetContainerFromItem<ListBoxItem>(endpoint);
                var root = VisualTree.FindFirstElementInVisualTree<Border>(item);

                if (index == 0)
                {
                    firstItemLocation = Canvas.GetLeft(item);
                }

                if (lastItem != null)
                {
                    endpoint.X = firstItemLocation + ((lastItem.ActualWidth + root.Margin.Left) * index);
                    endpoint.Y = lastEndpoint.Y;
                }

                lastItem = item;
                lastEndpoint = endpoint;
                index++;
            }
        }

    }

    public interface ILayoutManager
    {
        void PerformLayout(IDiagram diagram);
    }

}