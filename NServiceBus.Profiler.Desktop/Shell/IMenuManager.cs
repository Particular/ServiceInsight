using System.Windows;
using DevExpress.Xpf.Bars;
using NServiceBus.Profiler.Common.Plugins;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public interface IMenuManager
    {
        PopupMenu CreateContextMenu(FrameworkElement view);
        BarItem CreateContextMenuItem(PluginContextMenu pluginMenu);
    }
}