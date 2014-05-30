namespace Particular.ServiceInsight.Desktop.Explorer
{
    using System.Drawing;
    using Caliburn.Micro;

    public abstract class ExplorerItem : PropertyChangedBase
    {
        protected ExplorerItem(string name)
        {
            Children = new BindableCollection<ExplorerItem>();
            Name = name;
        }

        public bool IsExpanded { get; set; }

        public int TreeRowHandle { get; set; }

        public abstract Bitmap Image { get; }

        public string Name { get; private set; }

        public virtual string DisplayName { get { return Name; } }

        public IObservableCollection<ExplorerItem> Children { get; private set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}