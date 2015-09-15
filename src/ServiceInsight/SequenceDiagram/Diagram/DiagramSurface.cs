namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    public class DiagramSurface : Canvas
    {
        protected override Size MeasureOverride(Size constraint)
        {
            base.MeasureOverride(constraint);
            var desiredSize = new Size();

            foreach (UIElement child in Children)
            {
                var width = Math.Max(desiredSize.Width, GetLeft(child) + child.DesiredSize.Width);
                var height = Math.Max(desiredSize.Height, GetTop(child) + child.DesiredSize.Height);

                desiredSize = new Size(double.IsNaN(width) ? 0 : width,
                                       double.IsNaN(height) ? 0 : height);
            }

            return desiredSize;
        }

        public void Invalidate()
        {
            Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Arrange(new Rect(0, 0, DesiredSize.Width, DesiredSize.Height));
        }
    }
}