using Mindscape.WpfDiagramming;

namespace NServiceBus.Profiler.Desktop.MessageFlow
{
    public interface IConversationView
    {
        void ZoomToDefault();
        void ZoomToFill();
        void ApplyLayout();
        DiagramSurface Surface { get; }
    }
}