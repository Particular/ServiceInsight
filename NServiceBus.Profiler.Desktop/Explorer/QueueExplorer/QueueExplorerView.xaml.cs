using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using DevExpress.Xpf.Grid;
using NServiceBus.Profiler.Desktop.Events;

namespace NServiceBus.Profiler.Desktop.Explorer.QueueExplorer
{
    /// <summary>
    /// Interaction logic for QueuesListView.xaml
    /// </summary>
    public partial class QueueExplorerView : IExplorerView
    {
        public QueueExplorerView()
        {
            InitializeComponent();
        }

        private void OnFocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            var item = e.NewRow as ExplorerItem;
            Model.SelectedNode = item;
            Model.SelectedRowHandle = e.Source.FocusedRowHandle;
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

        private void OnLinkClicked(object sender, RoutedEventArgs e)
        {
            Model.Navigate(((Hyperlink)sender).NavigateUri.ToString());
        }
    }
}
