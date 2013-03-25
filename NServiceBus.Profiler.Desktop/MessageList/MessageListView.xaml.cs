using Caliburn.PresentationFramework.Actions;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
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
        }

        private readonly IMenuManager _menuManager;
        private PopupMenu _contextMenu;

        public MessageListView()
        {
            InitializeComponent();
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
            var storedMsg = Model.Messages[e.ListSourceRowIndex] as StoredMessage;

            if (e.IsGetData)
            {
                if (storedMsg != null && e.Column.FieldName == AdvancedEndpointColumns.CriticalTime)
                {
                    e.Value = Model.GetCriticalTime(storedMsg);
                }
                if (e.Column.FieldName == AdvancedEndpointColumns.IsFaulted)
                {
                    if (storedMsg != null)
                    {
                        e.Value = Model.GetMessageErrorInfo(storedMsg);
                    }
                    else
                    {
                        e.Value = Model.GetMessageErrorInfo();
                    }
                }
            }
        }

        private void OnBeforeLayoutRefresh(object sender, CancelRoutedEventArgs e)
        {
            e.Cancel = grid.ShowLoadingPanel;
        }
    }
}
