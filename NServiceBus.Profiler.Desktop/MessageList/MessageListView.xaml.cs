using System.Reflection;
using System.Windows;
using System.Windows.Input;
using DevExpress.Data;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid;
using NServiceBus.Profiler.Desktop.ExtensionMethods;
using NServiceBus.Profiler.Desktop.Models;
using NServiceBus.Profiler.Desktop.Shell;

namespace NServiceBus.Profiler.Desktop.MessageList
{
    /// <summary>
    /// Interaction logic for MessageListView.xaml
    /// </summary>
    public partial class MessageListView : IMessageListView
    {
        private static class AdvancedEndpointColumns
        {
            public const string CriticalTime = "CriticalTime";
            public const string ProcessingTime = "ProcessingTime";
            public const string IsFaulted = "IsFaulted";
            public const string MessageId = "Identifier";
        }

        private readonly IMenuManager _menuManager;
        private readonly PropertyInfo _sortUpProperty;
        private readonly PropertyInfo _sortDownProperty;

        public MessageListView()
        {
            InitializeComponent();
            _sortUpProperty = typeof (BaseGridColumnHeader).GetProperty("SortUpIndicator", BindingFlags.Instance | BindingFlags.NonPublic);
            _sortDownProperty = typeof (BaseGridColumnHeader).GetProperty("SortDownIndicator", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public MessageListView(IMenuManager menuManager)
            : this()
        {
            _menuManager = menuManager;
        }

        public void SetupContextMenu()
        {
            _menuManager.CreateContextMenu(grid.View, Model.ContextMenuItems);
        }

        private void OnFocusedMessageChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if (Model != null)
            {
                Model.FocusedMessage = e.NewRow as MessageInfo;
            }
        }

        private IMessageListViewModel Model
        {
            get { return (IMessageListViewModel) DataContext; }
        }

        private void OnSelectedMessagesChanged(object sender, GridSelectionChangedEventArgs e)
        {
            if (Model != null)
            {
                Model.SelectedMessages.Clear();

                foreach (var row in e.Source.SelectedRows)
                {
                    Model.SelectedMessages.Add((MessageInfo) row);
                }
            }
        }

        private void OnRequestAdvancedMessageData(object sender, GridColumnDataEventArgs e)
        {
            var msg = Model.Messages[e.ListSourceRowIndex];
            var storedMsg = msg as StoredMessage;

            if (e.IsGetData)
            {
                if (e.Column.FieldName == AdvancedEndpointColumns.MessageId)
                {
                    e.Value = storedMsg != null ? storedMsg.MessageId : msg.Id;
                }

                if (e.Column.FieldName == AdvancedEndpointColumns.CriticalTime)
                {
                    e.Value = Model.GetCriticalTime(storedMsg);
                }

                if (e.Column.FieldName == AdvancedEndpointColumns.ProcessingTime)
                {
                    e.Value = Model.GetProcessingTime(storedMsg);
                }

                if (e.Column.FieldName == AdvancedEndpointColumns.IsFaulted)
                {
                    e.Value = storedMsg != null ? Model.GetMessageErrorInfo(storedMsg) : Model.GetMessageErrorInfo();
                }
            }
        }

        private void OnBeforeLayoutRefresh(object sender, CancelRoutedEventArgs e)
        {
            e.Cancel = grid.ShowLoadingPanel;
        }

        private void SortData(ColumnBase column, ColumnSortOrder order)
        {
            Model.RefreshMessages(column.Tag as string, order == ColumnSortOrder.Ascending);
        }

        private void OnGridControlClicked(object sender, MouseButtonEventArgs e)
        {
            var columnHeader = LayoutHelper.FindLayoutOrVisualParentObject((DependencyObject) e.OriginalSource, typeof (GridColumnHeader)) as GridColumnHeader;
            if (columnHeader == null || Model == null || Model.WorkInProgress) return;

            var clickedColumn = (GridColumn) columnHeader.DataContext;
            if (clickedColumn.Tag == null) return;

            ClearSortExcept(columnHeader);

            var sortUpControl = (ColumnHeaderSortIndicatorControl) _sortUpProperty.GetValue(columnHeader, null);
            var sortDownControl = (ColumnHeaderSortIndicatorControl) _sortDownProperty.GetValue(columnHeader, null);
            ColumnSortOrder sort;

            if (sortUpControl.Visibility != Visibility.Visible)
            {
                sortUpControl.Visibility = Visibility.Visible;
                sortDownControl.Visibility = Visibility.Hidden;
                sort = ColumnSortOrder.Ascending;
            }
            else
            {
                sortUpControl.Visibility = Visibility.Hidden;
                sortDownControl.Visibility = Visibility.Visible;
                sort = ColumnSortOrder.Descending;
            }

            SortData(clickedColumn, sort);
        }

        private void HideIndicator(BaseGridColumnHeader header)
        {
            var sortUpControl = (ColumnHeaderSortIndicatorControl) _sortUpProperty.GetValue(header, null);
            var sortDownControl = (ColumnHeaderSortIndicatorControl) _sortDownProperty.GetValue(header, null);

            sortUpControl.Visibility = Visibility.Hidden;
            sortDownControl.Visibility = Visibility.Hidden;
        }

        private void ClearSortExcept(GridColumnHeader clickedHeader)
        {
            var headers = grid.FindVisualChildren<GridColumnHeader>();
            foreach (var header in headers)
            {
                if (header == clickedHeader)
                    continue;

                HideIndicator(header);
            }
        }

        public TableView Table
        {
            get { return (TableView)grid.View; }
        }
    }
}
