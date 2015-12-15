namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System.Windows;

    public interface IDiagram
    {
        DiagramItemCollection DiagramItems { get; }

        DiagramVisualItem GetItemFromContainer(DiagramItem item);

        Thickness Padding { get; set; }
    }
}