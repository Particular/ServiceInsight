using System.Windows;
using Caliburn.PresentationFramework;
using Particular.ServiceInsight.Desktop.MessageList;

namespace Particular.ServiceInsight.Desktop.Shell
{
    public interface IMenuManager
    {
        void CreateContextMenu(FrameworkElement view, IObservableCollection<ContextMenuModel> contextMenuItems);
    }
}