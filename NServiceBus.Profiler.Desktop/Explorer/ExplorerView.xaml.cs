using System;
using DevExpress.Xpf.Grid;
using NServiceBus.Profiler.Common.Events;

namespace NServiceBus.Profiler.Desktop.Explorer
{
    /// <summary>
    /// Interaction logic for QueuesListView.xaml
    /// </summary>
    public partial class ExplorerView : IExplorerView
    {
        public ExplorerView()
        {
            InitializeComponent();
        }

        private void OnFocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            var item = e.NewRow as ExplorerItem;
            Model.SelectedNode = item;
            Model.SelectedRowHandle = e.Source.FocusedRowHandle;
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

        public void Handle(WorkStartedEvent message)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                treeList.IsHitTestVisible = false;
                treeList.ShowLoadingPanel = true;
            }));
        }

        public void Handle(WorkFinishedEvent message)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                treeList.ShowLoadingPanel = false;
                treeList.IsHitTestVisible = true;
            }));
        }
    }
}
