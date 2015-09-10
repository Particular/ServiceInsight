namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System.Linq;

    public class SequenceDiagramLayoutManager : ILayoutManager
    {
        public void PerformLayout(IDiagram diagram)
        {
            LayoutEndpoints(diagram);
        }

        private void LayoutEndpoints(IDiagram diagram)
        {
            if (diagram.Items == null || diagram.Items.Count == 0) return;
            
            var endpoints = diagram.Items.OfType<EndpointItem>().ToList();
            
            EndpointItem lastEndpoint = null;
            var firstX = 0d;
            var index = 0;
            
            foreach (var endpoint in endpoints)
            {
                if (index == 0)
                {
                    firstX = endpoint.X;
                }
            
                if (lastEndpoint != null)
                {
                    endpoint.X = firstX + ((lastEndpoint.Width + 10 /*Margin*/) * index);
                    endpoint.Y = lastEndpoint.Y;
                }
            
                lastEndpoint = endpoint;
                index++;
            }



            //            if (diagram.Items == null || diagram.Items.Count == 0) return;
            //
            //            var endpoints = diagram.Items.OfType<EndpointItem>().ToList();
            //
            //            ListBoxItem lastItem = null;
            //            EndpointItem lastEndpoint = null;
            //            var firstItemLocation = 0d;
            //            var index = 0;
            //
            //            foreach (var endpoint in endpoints)
            //            {
            //
            //                var root = VisualTree.FindFirstElementInVisualTree<Border>(item);
            //                if (root == null)
            //                    continue;
            //
            //                if (index == 0)
            //                {
            //                    firstItemLocation = Canvas.GetLeft(item);
            //                }
            //
            //                if (lastItem != null)
            //                {
            //                    endpoint.X = firstItemLocation + ((lastItem.ActualWidth + root.Margin.Left) * index);
            //                    endpoint.Y = lastEndpoint.Y;
            //                }
            //
            //                lastItem = item;
            //                lastEndpoint = endpoint;
            //                index++;
            //            }
        }

    }

    public interface ILayoutManager
    {
        void PerformLayout(IDiagram diagram);
    }

}