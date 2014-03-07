using Mindscape.WpfDiagramming;
using Mindscape.WpfDiagramming.Foundation;
using System;

namespace NServiceBus.Profiler.Desktop.MessageFlow
{
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