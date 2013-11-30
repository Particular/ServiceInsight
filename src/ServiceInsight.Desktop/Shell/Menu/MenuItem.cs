using System.Drawing;
using System.Windows.Input;

namespace NServiceBus.Profiler.Desktop.Shell.Menu
{
    public class MenuItem : MenuItemBase
    {
        //TODO: Use caliburn actions and messages
        public MenuItem(string caption, ICommand command, Bitmap icon = null, bool isCheckable = false)
            : base(caption, false, isCheckable)
        {
            Command = command;
            Icon = icon;
        }
    }
}