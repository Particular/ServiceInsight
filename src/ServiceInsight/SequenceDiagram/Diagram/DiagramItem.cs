namespace ServiceInsight.SequenceDiagram.Diagram
{
    using Pirac;

    public abstract class DiagramItem : BindableObject
    {
        public string Name { get; set; }

        public bool IsFocused { get; set; }

        protected virtual void OnIsFocusedChanged()
        {
        }

        public void Refresh()
        {
            OnPropertyChanging("", null);
            OnPropertyChanged("", null, null);
        }
    }
}