using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;

namespace NServiceBus.Profiler.Desktop.Shell
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class ShellView : IShellView
    {
        public ShellView()
        {
            ChangeTheme(Theme.VS2010Name);
            InitializeComponent();
        }

        public void ChangeTheme(string name)
        {
            ThemeManager.ApplicationThemeName = name;
        }

        public BarManager GetMenuManager()
        { 
            return BarManager;
        }
    }
}
