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
                desiredSize = new Size(
                    Math.Max(desiredSize.Width, GetLeft(child) + child.DesiredSize.Width),
                    Math.Max(desiredSize.Height, GetTop(child) + child.DesiredSize.Height));
            }

            return desiredSize;
        }
    }
}