namespace Particular.ServiceInsight.Desktop.MessageFlow
{
    using Mindscape.WpfDiagramming.FlowDiagrams;

    public class MessageFlowDiagram : FlowDiagramModel
    {
        public MessageFlowDiagram()
        {
            DefaultConnectionBuilder = new Mindscape.WpfDiagramming.FlowDiagrams.FlowDiagramConnectionBuilder();
        }
    }
}