using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;
using Caliburn.PresentationFramework;

namespace NServiceBus.Profiler.Desktop.Shell.Menu
{
    public abstract class MenuItemBase : PropertyChangedBase, IMenuItem
    {
        protected MenuItemBase(string caption, bool isSeparator, bool isCheckable = false)
        {
            Caption = caption;
            IsSeparator = isSeparator;
            IsCheckable = isCheckable;
            IsEnabled = true;
            IsVisible = true;
            CloseSubMenuOnClick = true;
            IsChecked = false;
            KeyGesture = string.Empty;
            SubMenuItems = new List<IMenuItem>();
        }

        public IList<IMenuItem> SubMenuItems { get; private set; }

        //TODO: Use caliburn actions and messages
        public ICommand Command { get; set; }

        public bool IsVisible { get; set; }
        public bool IsEnabled { get; set; }
        public string Caption { get; private set; }
        public Bitmap Icon { get; set; }
        public bool IsCheckable { get; private set; }
        public bool IsChecked { get; set; }
        public bool IsSeparator { get; private set; }
        public bool CloseSubMenuOnClick { get; private set; }
        public string ToolTip { get; private set; }
        public string KeyGesture { get; private set; }
        public object Tag { get; set; }
    }
}