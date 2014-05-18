using Mindscape.WpfDiagramming.FlowDiagrams;

namespace NServiceBus.Profiler.Desktop.MessageSequenceDiagram
{
    public class MessageSequenceDiagram : FlowDiagramModel
    {
        public MessageSequenceDiagram()
        {
            DefaultConnectionBuilder = new SequenceDiagramConnectionBuilder();
        }
    }
}
