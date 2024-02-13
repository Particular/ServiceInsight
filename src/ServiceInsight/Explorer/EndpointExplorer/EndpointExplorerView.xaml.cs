namespace ServiceInsight.Explorer.EndpointExplorer
{
    public interface IEndpointExplorerView
    {
        void ExpandSelectedNode();
    }

    public partial class EndpointExplorerView : IEndpointExplorerView
    {
        public EndpointExplorerView()
        {
            InitializeComponent();
        }

        public void ExpandSelectedNode()
        {
            var rowHandle = treeView.GetSelectedRowHandles();
            if (rowHandle.Length > 0)
            {
                treeView.ExpandNode(rowHandle[0]);
            }
        }
    }
}