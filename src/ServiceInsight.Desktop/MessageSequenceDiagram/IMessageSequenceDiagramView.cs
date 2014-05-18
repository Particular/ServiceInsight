using Mindscape.WpfDiagramming;
using Mindscape.WpfDiagramming.Foundation;
using System;

namespace NServiceBus.Profiler.Desktop.MessageSequenceDiagram
{
    public interface IMessageSequenceDiagramView
    {
        void ApplyLayout();
        DiagramSurface Surface { get; }
        void SizeToFit();

        //event EventHandler<SearchMessageEventArgs> ShowMessage;
    }
}
