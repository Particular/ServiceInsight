using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;

namespace NServiceBus.Profiler.Desktop.Shell.Menu
{
    public interface IMenuItem
    {
        string ToolTip { get; }
        bool IsVisible { get; set; }
        bool IsEnabled { get; set; }
        string Caption { get; }
        IList<IMenuItem> SubMenuItems { get; }
        Bitmap Icon { get; set; }
        bool IsCheckable { get; }
        bool IsChecked { get; set; }
        bool IsSeparator { get; }
        bool CloseSubMenuOnClick { get; }
        ICommand Command { get; set; }
        string KeyGesture { get; }
        object Tag { get; set; }
    }
}