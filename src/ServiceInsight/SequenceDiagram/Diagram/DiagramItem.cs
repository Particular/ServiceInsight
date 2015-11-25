namespace ServiceInsight.SequenceDiagram.Diagram
{
    public abstract class DiagramItem : Caliburn.Micro.PropertyChangedBase
    {
        public string Name { get; set; }

        public bool IsFocused { get; set; }

        public bool ShouldBringIntoView { get; set; }

        public virtual void OnIsFocusedChanged()
        {
        }
    }
}