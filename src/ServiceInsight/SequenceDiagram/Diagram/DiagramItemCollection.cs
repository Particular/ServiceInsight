namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    public sealed class DiagramItemCollection : ObservableCollection<DiagramItem>
    {
        bool _suppressNotification;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotification)
            {
                base.OnCollectionChanged(e);
            }
        }

        public void AddRange(IEnumerable<DiagramItem> items)
        {
            if (items == null) throw new ArgumentNullException("items");

            try
            {
                _suppressNotification = true;

                foreach (var item in items)
                {
                    Add(item);
                }
            }
            finally
            {
                _suppressNotification = false;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }
    }
}