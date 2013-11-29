using System;
using System.Windows.Input;
using NServiceBus.Profiler.Desktop.Events;

namespace NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer
{
    /// <summary>
    /// Interaction logic for QueuesListView.xaml
    /// </summary>
    public partial class EndpointExplorerView : IExplorerView
    {
        public EndpointExplorerView()
        {
            InitializeComponent();
        }

        private void OnTreeClicked(object sender, MouseButtonEventArgs e)
        {
            Model.OnSelectedNodeChanged();
        }

        private IExplorerViewModel Model
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

        public void Handle(AsyncOperationFailedEvent message)
        {
            Dispatcher.BeginInvoke((Action)(StopWorkInProgress));
        }

        private void StopWorkInProgress()
        {
            treeList.ShowLoadingPanel = false;
            treeList.IsHitTestVisible = true;
        }

        private void StartWorkInProgress()
        {
            treeList.IsHitTestVisible = false;
            treeList.ShowLoadingPanel = true;
        }
    }
}
