namespace ServiceInsight.Explorer
{
    using System.Drawing;
    using Caliburn.Micro;
    using EndpointExplorer;

    public abstract class ExplorerItem : PropertyChangedBase
    {
        protected ExplorerItem(string name)
        {
            Children = new BindableCollection<EndpointExplorerItem>();
            Name = name;
        }

        public bool IsExpanded { get; set; }

        public int TreeRowHandle { get; set; }

        public abstract Bitmap Image { get; }

        public string Name { get; private set; }

        public virtual string DisplayName { get { return Name; } }

        public IObservableCollection<EndpointExplorerItem> Children { get; private set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}