using System.Windows;
using Caliburn.PresentationFramework;
using NServiceBus.Profiler.Desktop.MessageList;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public interface IMenuManager
    {
        void CreateContextMenu(FrameworkElement view, IObservableCollection<ContextMenuModel> contextMenuItems);
    }
}