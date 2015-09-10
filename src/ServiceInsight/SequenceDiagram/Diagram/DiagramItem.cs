namespace ServiceInsight.SequenceDiagram.Diagram
{
    public abstract class DiagramItem : Caliburn.Micro.PropertyChangedBase
    {
        public string Name { get; set; }

        public virtual double X { get; set; }

        public virtual double Y { get; set; }

        public virtual double Width { get; set; }

        public virtual double Height { get; set; }
    }
}