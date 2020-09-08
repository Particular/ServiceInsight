namespace ServiceInsight.MessageList
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using DevExpress.Xpf.Core;
    using DevExpress.Xpf.Grid;
    using ServiceInsight.ExtensionMethods;
    using ServiceInsight.Framework.Settings;
    using ServiceInsight.Settings;
    using ServiceInsight.Shell;

    public interface IMessageListView : IPersistableLayout
    {
        void BeginDataUpdate();

        void EndDataUpdate();
    }

    public partial class MessageListView : IMessageListView
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

        MessageListViewModel Model => (MessageListViewModel)DataContext;

        void OnRequestAdvancedMessageData(object sender, GridColumnDataEventArgs e)
        {
            var msg = Model.Rows[e.ListSourceRowIndex];

            if (e.IsGetData && e.Column.FieldName == UnboundColumns.IsFaulted)
            {
                e.Value = msg.MessageStatusInfo;
            }
        }

        void OnBeforeLayoutRefresh(object sender, CancelRoutedEventArgs e)
        {
            e.Cancel = grid.ShowLoadingPanel;
        }

        async Task SortData(ColumnBase column, ListSortDirection order)
        {
            var orderBy = column.Tag as string;
            var ascending = order == ListSortDirection.Ascending;

            await Model.RefreshMessages(orderBy, ascending);
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

        async void Grid_OnStartSorting(object sender, RoutedEventArgs e)
        {
            var grid = e.Source as GridControl;

            if (grid == null || Model == null || Model.WorkInProgress)
            {
                return;
            }

            var sortInfo = grid.SortInfo.FirstOrDefault();

            if (sortInfo == null)
            {
                return;
            }

            var column = grid.Columns.First(c => c.FieldName == sortInfo.FieldName);

            await SortData(column, sortInfo.SortOrder);

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

        public void OnSaveLayout(ISettingsProvider settingsProvider)
        {
            var layoutSetting = settingsProvider.GetSettings<MessageListSettings>();
            layoutSetting.GridLayout = grid.GetLayout();
            settingsProvider.SaveSettings(layoutSetting);
        }

        public void OnRestoreLayout(ISettingsProvider settingsProvider)
        {
            var layoutSetting = settingsProvider.GetSettings<MessageListSettings>();
            grid.RestoreLayout(layoutSetting.GridLayout.GetAsStream());
        }

        public void OnResetLayout(ISettingsProvider settingsProvider)
        {
            var layoutSettings = settingsProvider.GetSettings<MessageListSettings>();
            layoutSettings.GridLayout = null;
            settingsProvider.SaveSettings(layoutSettings);
        }
    }
}