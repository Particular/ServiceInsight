using Mindscape.WpfDiagramming.FlowDiagrams;

namespace NServiceBus.Profiler.Desktop.MessageFlow
{
    public class MessageFlowDiagram : FlowDiagramModel
    {
        public MessageFlowDiagram()
        {
            DefaultConnectionBuilder = new FlowDiagramConnectionBuilder();
        }
    }
}