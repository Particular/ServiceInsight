using System.Windows;
using DevExpress.Xpf.Bars;
using NServiceBus.Profiler.Desktop.MessageList;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public interface IMenuManager
    {
        PopupMenu CreateContextMenu(FrameworkElement view);
        BarItem CreateContextMenuItem(ContextMenuModel menu);
    }
}