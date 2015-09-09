namespace Particular.ServiceInsight.Desktop.SequenceDiagram.Diagram
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    public sealed class DiagramItemCollection : ObservableCollection<DiagramItem>
    {
        public new event NotifyCollectionChangedEventHandler CollectionChanged;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, e);
            }
        }

        public void AddRange(params DiagramItem[] items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }
    }
}