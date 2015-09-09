namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System.Windows;

    public interface IDiagram
    {
        DiagramItemCollection Items { get; }
        T GetContainerFromItem<T>(DiagramItem item) where T : UIElement;
    }
}