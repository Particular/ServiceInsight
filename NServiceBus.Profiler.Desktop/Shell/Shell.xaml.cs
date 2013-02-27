using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.NavBar;
using NServiceBus.Profiler.Desktop.MessageHeaders;

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

        private void OnGroupAdding(object sender, GroupAddingEventArgs e)
        {
            if (e.Group == null)
                return;

            var headerScreen = e.Group.DataContext as IHeaderInfoViewModel;
            if(headerScreen == null)
                return;

            e.Group.Header = headerScreen.DisplayName;
            e.Group.ImageSource = headerScreen.GroupImage;
            e.Group.ItemsSource = headerScreen.Items;
        }
    }
}
