namespace ServiceInsight.SequenceDiagram.Drawing
{
    public class ArrowViewModel : UmlViewModel
    {
        public HandlerViewModel From { get; set; }
        public HandlerViewModel To { get; set; }
        public ArrowType Type { get; set; }

    }
}