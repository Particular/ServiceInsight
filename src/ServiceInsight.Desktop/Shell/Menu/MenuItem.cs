namespace Particular.ServiceInsight.Desktop.Shell.Menu
{
    using System.Drawing;
    using System.Windows.Input;

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