namespace ServiceInsight.Shell
{
    using System.Windows;
    using DevExpress.Xpf.Bars;
    using DevExpress.Xpf.Core;
    using DevExpress.Xpf.Core.Serialization;
    using DevExpress.Xpf.Docking;
    using DevExpress.Xpf.Utils.Themes;
    using ExtensionMethods;
    using ServiceInsight.Framework.Settings;
    using Settings;

    public partial class ShellView : IShellView
    {
        public ShellView()
        {
            ChangeTheme(Theme.Office2013Name);
            InitializeComponent();
            BarManager.CheckBarItemNames = false;
            Loaded += OnShellLoaded;
        }

        void OnShellLoaded(object sender, RoutedEventArgs e)
        {
            if (DXSplashScreen.IsActive)
            {
                DXSplashScreen.Close();
            }

            Activate();

            OpenClosedPanels();
        }

        void OpenClosedPanels()
        {
            foreach (var panel in DockManager.ClosedPanels.ToArray())
            {
                DockManager.DockController.Restore(panel);
            }
        }

        public void ChangeTheme(string name)
        {
            GlobalThemeHelper.Instance.ApplicationThemeName = name;
        }

        public void SelectTab(string name)
        {
            var tab = DockManager.GetItem(name);
            if (tab != null)
            {
                DockManager.Activate(tab);
            }
        }

        public void OnSaveLayout(ISettingsProvider settingProvider)
        {
            var layoutSetting = settingProvider.GetSettings<ShellLayoutSettings>();

            if (!layoutSetting.ResetLayout)
            {
                layoutSetting.LayoutVersion = GetCurrentLayoutVersion();
                layoutSetting.DockLayout = DockManager.GetLayout();
                layoutSetting.MenuLayout = BarManager.GetLayout();
                layoutSetting.MainWindowHeight = Height;
                layoutSetting.MainWindowWidth = Width;
                layoutSetting.MainWindowTop = Top;
                layoutSetting.MainWindowLeft = Left;
                layoutSetting.MainWindowState = WindowState;
            }
            else
            {
                layoutSetting.ResetLayout = false;
            }

            settingProvider.SaveSettings(layoutSetting);
        }

        public void OnRestoreLayout(ISettingsProvider settingsProvider)
        {
            var layoutSetting = settingsProvider.GetSettings<ShellLayoutSettings>();
            var currentLayoutVersion = GetCurrentLayoutVersion();

            if (layoutSetting.LayoutVersion == currentLayoutVersion)
            {
                DockManager.RestoreLayout(layoutSetting.DockLayout.GetAsStream());
                BarManager.RestoreLayout(layoutSetting.MenuLayout.GetAsStream());
            }

            Top = layoutSetting.MainWindowTop;
            Left = layoutSetting.MainWindowLeft;
            Width = layoutSetting.MainWindowWidth;
            Height = layoutSetting.MainWindowHeight;
            WindowState = layoutSetting.MainWindowState;
        }

        public void OnResetLayout(ISettingsProvider settingsProvider)
        {
            var layoutSettings = settingsProvider.GetSettings<ShellLayoutSettings>();

            layoutSettings.ResetLayout = true;
            layoutSettings.MenuLayout = null;
            layoutSettings.DockLayout = null;
            layoutSettings.MainWindowState = WindowState.Maximized;
            layoutSettings.MainWindowTop = SystemParameters.VirtualScreenTop;
            layoutSettings.MainWindowLeft = SystemParameters.VirtualScreenLeft;
            layoutSettings.MainWindowWidth = SystemParameters.VirtualScreenWidth;
            layoutSettings.MainWindowHeight = SystemParameters.VirtualScreenHeight;

            settingsProvider.SaveSettings(layoutSettings);

            OnRestoreLayout(settingsProvider);
        }

        string GetCurrentLayoutVersion() => DXSerializer.GetLayoutVersion(BarManager);

        ShellViewModel Model => DataContext as ShellViewModel;

        void MessageBody_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Model != null)
            {
                Model.BodyTabSelected = (bool)e.NewValue;
            }
        }
    }
}