namespace ServiceInsight.SequenceDiagram.Diagram
{
    public abstract class DiagramItem : Caliburn.Micro.PropertyChangedBase
    {
        public string Name { get; set; }

        public bool IsFocused { get; set; }

        protected virtual void OnIsFocusedChanged()
        {
        }
    }
}