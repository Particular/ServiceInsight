using System.Windows;
using DevExpress.Xpf.Bars;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public class MenuManager : IMenuManager
    {
        private readonly IShellView _shell;

        public MenuManager(IShellView shell)
        {
            _shell = shell;
        }

        public PopupMenu CreateContextMenu(FrameworkElement view)
        {
            var contextMenu = new PopupMenu { Manager = _shell.GetMenuManager() };
            BarManager.SetDXContextMenu(view, contextMenu);

            return contextMenu;
        }
    }
}