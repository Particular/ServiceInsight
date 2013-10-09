using Mindscape.WpfDiagramming;

namespace NServiceBus.Profiler.Desktop.MessageFlow
{
    public interface IMessageFlowView
    {
        void ZoomToDefault();
        void ZoomToFill();
        void ApplyLayout();
        DiagramSurface Surface { get; }
    }
}