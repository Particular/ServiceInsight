namespace Particular.ServiceInsight.Desktop.Explorer.EndpointExplorer
{
    using System;
    using System.Windows.Input;
    using Events;

    /// <summary>
    /// Interaction logic for QueuesListView.xaml
    /// </summary>
    public partial class EndpointExplorerView : IExplorerView
    {
        public EndpointExplorerView()
        {
            InitializeComponent();
        }

        void OnTreeClicked(object sender, MouseButtonEventArgs e)
        {
            Model.OnSelectedNodeChanged();
        }

        IExplorerViewModel Model
        {
            get { return DataContext as IExplorerViewModel; }
        }

        public void SelectRow(int rowHandle)
        {
            view.FocusedRowHandle = rowHandle;
        }

        public void ExpandNode(ExplorerItem item)
        {
            var node = treeList.View.GetNodeByContent(item);
            if (node != null)
            {
                view.ExpandNode(node.RowHandle);
            }
        }

        public void Expand()
        {
            if (view != null)
            {
                view.ExpandAllNodes();
            }
        }

        public void Handle(WorkStarted message)
        {
            Dispatcher.BeginInvoke((Action)(StartWorkInProgress));
        }

        public void Handle(WorkFinished message)
        {
            Dispatcher.BeginInvoke((Action)(StopWorkInProgress));
        }

        public void Handle(AsyncOperationFailed message)
        {
            Dispatcher.BeginInvoke((Action)(StopWorkInProgress));
        }

        void StopWorkInProgress()
        {
            if (Model.Parent.AutoRefresh) return;

            treeList.ShowLoadingPanel = false;
            treeList.IsHitTestVisible = true;
        }

        void StartWorkInProgress()
        {
            if(Model.Parent.AutoRefresh) return;

            treeList.IsHitTestVisible = false;
            treeList.ShowLoadingPanel = true;
        }
    }
}
