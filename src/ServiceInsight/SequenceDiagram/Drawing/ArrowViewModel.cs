namespace ServiceInsight.SequenceDiagram.Drawing
{
    public class ArrowViewModel : UmlViewModel
    {
        public HandlerViewModel From { get; set; }
        public HandlerViewModel To { get; set; }

        public int Vector { get; set; }
        public int Length { get; set; }
        public ArrowType Type { get; set; }

    }
}