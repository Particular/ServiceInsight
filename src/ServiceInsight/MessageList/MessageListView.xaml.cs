namespace ServiceInsight.MessageList
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using DevExpress.Xpf.Core;
    using DevExpress.Xpf.Grid;
    using ServiceInsight.Models;

    public partial class MessageListView
    {
        static class UnboundColumns
        {
            public const string IsFaulted = "IsFaulted";
        }

        public MessageListView()
        {
            InitializeComponent();
        }

        public void BeginDataUpdate()
        {
            grid.BeginDataUpdate();
        }

        public void EndDataUpdate()
        {
            grid.EndDataUpdate();
        }

        void GridOnCurrentItemChanged(object sender, CurrentItemChangedEventArgs currentItemChangedEventArgs)
        {
            var msg = currentItemChangedEventArgs.NewItem as StoredMessage;
            Model.RaiseSelectedMessageChanged(msg);
        }

        MessageListViewModel Model => (MessageListViewModel)DataContext;

        void OnRequestAdvancedMessageData(object sender, GridColumnDataEventArgs e)
        {
            var msg = Model.Rows[e.ListSourceRowIndex];

            if (e.IsGetData && e.Column.FieldName == UnboundColumns.IsFaulted)
            {
                e.Value = Model.GetMessageErrorInfo(msg);
            }
        }

        void OnBeforeLayoutRefresh(object sender, CancelRoutedEventArgs e)
        {
            e.Cancel = grid.ShowLoadingPanel;
        }

        void SortData(ColumnBase column, ListSortDirection order)
        {
            Model.RefreshMessages(column.Tag as string, order == ListSortDirection.Ascending);
        }

        void Grid_OnCustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Value is TimeSpan)
            {
                e.DisplayText = ((TimeSpan)e.Value).SubmillisecondHumanize();
            }
            if (e.Value is DateTime? && (DateTime?)e.Value == DateTime.MinValue)
            {
                e.DisplayText = "Unknown";
            }
        }

        void Grid_OnStartSorting(object sender, RoutedEventArgs e)
        {
            var sourceGrid = e.Source as GridControl;

            if (sourceGrid == null || Model == null || Model.WorkInProgress)
            {
                return;
            }

            var sortInfo = sourceGrid.SortInfo.FirstOrDefault();

            if (sortInfo == null)
            {
                return;
            }

            var column = sourceGrid.Columns.First(c => c.FieldName == sortInfo.FieldName);

            SortData(column, sortInfo.SortOrder);

            e.Handled = true;
        }

        void OnGridClicked(object sender, MouseButtonEventArgs e)
        {
            var info = ((TableView)grid.View).CalcHitInfo(e.OriginalSource as DependencyObject);
            if (info.HitTest == TableViewHitTest.RowCell)
            {
                Model.BringIntoView(Model.Selection.SelectedMessage);
            }
        }
    }
}