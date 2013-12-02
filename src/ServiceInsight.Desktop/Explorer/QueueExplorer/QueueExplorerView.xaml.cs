using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
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

        public void Handle(AsyncOperationFailed message)
        {
            Dispatcher.BeginInvoke((Action)(StopWorkInProgress));
        }

        private void StopWorkInProgress()
        {
            if (Model.Parent.AutoRefresh) return;

            treeList.ShowLoadingPanel = false;
            treeList.IsHitTestVisible = true;
        }

        private void StartWorkInProgress()
        {
            if(Model.Parent.AutoRefresh) return;

            treeList.IsHitTestVisible = false;
            treeList.ShowLoadingPanel = true;
        }

        private void OnLinkClicked(object sender, RoutedEventArgs e)
        {
            Model.Navigate(((Hyperlink)sender).NavigateUri.ToString());
        }
    }
}
