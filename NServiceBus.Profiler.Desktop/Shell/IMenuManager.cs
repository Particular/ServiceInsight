using System.Windows;
using DevExpress.Xpf.Bars;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public interface IMenuManager
    {
        PopupMenu CreateContextMenu(FrameworkElement view);
    }
}