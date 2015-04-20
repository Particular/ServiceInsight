namespace Particular.ServiceInsight.Desktop.MessageFlow
{
    using System;
    using Mindscape.WpfDiagramming;
    using Mindscape.WpfDiagramming.Foundation;

    public interface IMessageFlowView
    {
        void ApplyLayout();
        DiagramSurface Surface { get; }
        void UpdateNode(IDiagramNode node);
        void UpdateConnections();
        void SizeToFit();

        event EventHandler<SearchMessageEventArgs> ShowMessage;
    }
}