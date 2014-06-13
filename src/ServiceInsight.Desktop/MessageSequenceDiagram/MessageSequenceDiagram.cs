using Mindscape.WpfDiagramming.FlowDiagrams;

namespace Particular.ServiceInsight.Desktop.MessageSequenceDiagram
{
    public class MessageSequenceDiagram : FlowDiagramModel
    {
        public MessageSequenceDiagram()
        {
            DefaultConnectionBuilder = new SequenceDiagramConnectionBuilder();
        }
    }
}
