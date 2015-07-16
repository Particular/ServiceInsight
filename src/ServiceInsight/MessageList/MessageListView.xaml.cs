namespace Particular.ServiceInsight.Desktop.MessageList
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using DevExpress.Data;
    using DevExpress.Xpf.Core;
    using DevExpress.Xpf.Grid;
    using Particular.ServiceInsight.Desktop.Models;

    public partial class MessageListView
    {
        static class UnboundColumns
        {
            public const string IsFaulted = "IsFaulted";
        }

        public MessageListView()
        {
            InitializeComponent();
            grid.CurrentItemChanged += GridOnCurrentItemChanged;
        }

        void GridOnCurrentItemChanged(object sender, CurrentItemChangedEventArgs currentItemChangedEventArgs)
        {
            Model.RaiseSelectedMessageChanged(currentItemChangedEventArgs.NewItem as StoredMessage);
        }

        MessageListViewModel Model
        {
            get { return (MessageListViewModel)DataContext; }
        }

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

        DataController Controller
        {
            get { return grid.DataController; }
        }

        public void BeginSelection()
        {
            Controller.Selection.BeginSelection();
        }

        public void EndSelection()
        {
            Controller.Selection.EndSelection();
        }

        void Grid_OnCustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Value is TimeSpan)
            {
                e.DisplayText = ((TimeSpan)e.Value).SubmillisecondHumanize();
            }
        }

        void Grid_OnStartSorting(object sender, RoutedEventArgs e)
        {
            var grid = e.Source as GridControl;

            if (grid == null || Model == null || Model.WorkInProgress) return;

            var sortInfo = grid.SortInfo.FirstOrDefault();

            if (sortInfo == null) return;

            var column = grid.Columns.First(c => c.FieldName == sortInfo.FieldName);

            SortData(column, sortInfo.SortOrder);

            e.Handled = true;
        }
    }
}