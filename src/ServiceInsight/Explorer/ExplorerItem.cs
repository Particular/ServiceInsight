namespace ServiceInsight.Explorer
{
    using System.Collections.Generic;
    using System.Drawing;

    using EndpointExplorer;
    using Pirac;

    public abstract class ExplorerItem : BindableObject
    {
        protected ExplorerItem(string name)
        {
            Children = new List<EndpointExplorerItem>();
            Name = name;
        }

        public bool IsExpanded { get; set; }

        public int TreeRowHandle { get; set; }

        public abstract Bitmap Image { get; }

        public string Name { get; }

        public virtual string DisplayName => Name;

        public IList<EndpointExplorerItem> Children { get; }

        public override string ToString() => DisplayName;
    }
}