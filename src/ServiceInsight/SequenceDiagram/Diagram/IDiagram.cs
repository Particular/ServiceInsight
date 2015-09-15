namespace ServiceInsight.SequenceDiagram.Diagram
{
    public interface IDiagram
    {
        DiagramItemCollection DiagramItems { get; }
        DiagramVisualItem GetItemFromContainer(DiagramItem item);
    }
}