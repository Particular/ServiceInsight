namespace Particular.ServiceInsight.Desktop.MessageSequenceDiagram
{
    using Mindscape.WpfDiagramming;
    using Mindscape.WpfDiagramming.Foundation;
    using System;
    
    public interface IMessageSequenceDiagramView
    {
        void ApplyLayout();
        DiagramSurface Surface { get; }
        void SizeToFit();

        //event EventHandler<SearchMessageEventArgs> ShowMessage;
    }
}
