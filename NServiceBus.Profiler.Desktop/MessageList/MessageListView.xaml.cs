using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.PresentationFramework.Actions;
using DevExpress.Data;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid;
using NServiceBus.Profiler.Common.ExtensionMethods;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Desktop.Shell;
using Message = Caliburn.PresentationFramework.RoutedMessaging.Message;

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
            public const string IsFaulted = "IsFaulted";
            public const string MessageId = "Identifier";
        }

        private readonly IMenuManager _menuManager;
        private PopupMenu _contextMenu;
        private PropertyInfo _sortUpProperty;
        private PropertyInfo _sortDownProperty;

        public MessageListView()
        {
            InitializeComponent();
//            _sortUpProperty = typeof(BaseGridColumnHeader).GetProperty("SortUpIndicator", BindingFlags.Instance | BindingFlags.NonPublic);
//            _sortDownProperty = typeof(BaseGridColumnHeader).GetProperty("SortDownIndicator", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public MessageListView(IMenuManager menuManager)
            : this()
        {
            _menuManager = menuManager;
        }

        public void SetupContextMenu()
        {
            _contextMenu = _menuManager.CreateContextMenu(grid.View);
            
            foreach (var item in Model.ContextMenuItems)
            {
                var menu = _menuManager.CreateContextMenuItem(item);
                
                Message.SetAttach(menu, string.Format("[Event ItemClick]=[Action {0}]", item.Name));
                Action.SetTarget(menu, Model);

                _contextMenu.ItemLinks.Add(menu);
            }
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
            get { return (IMessageListViewModel)DataContext; }
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

                if (e.Column.FieldName == AdvancedEndpointColumns.CriticalTime && storedMsg != null)
                {
                    e.Value = Model.GetCriticalTime(storedMsg);
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

        private void OnColumnHeaderClicked(object sender, ColumnHeaderClickEventArgs e)
        {
            Model.RefreshMessages(e.Column.Tag as string, e.Column.SortOrder == ColumnSortOrder.Ascending);
        }
    }
}
