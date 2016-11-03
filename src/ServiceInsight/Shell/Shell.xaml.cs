namespace ServiceInsight.Shell
{
    using System;
    using System.IO;
    using System.Windows;
    using Anotar.Serilog;
    using DevExpress.Xpf.Bars;
    using DevExpress.Xpf.Core;
    using DevExpress.Xpf.Core.Serialization;
    using DevExpress.Xpf.Docking;
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
            ThemeManager.ApplicationThemeName = name;
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
                layoutSetting.DockLayout = GetLayout(DockManager);
                layoutSetting.MenuLayout = GetLayout(BarManager);
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
                SetLayout(DockManager, layoutSetting.DockLayout.GetAsStream());
                SetLayout(BarManager, layoutSetting.MenuLayout.GetAsStream());
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

        string GetLayout(dynamic control) //Lack of common interface :(
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    control.SaveLayoutToStream(ms);
                    return ms.GetAsString();
                }
            }
            catch (Exception ex)
            {
                LogTo.Information(ex, "Failed to save the layout, reason is: {ex}", ex);
                return null;
            }
        }

        void SetLayout(dynamic control, Stream layout)
        {
            if (layout == null)
            {
                return;
            }

            try
            {
                control.RestoreLayoutFromStream(layout);
            }
            catch (Exception ex)
            {
                LogTo.Information(ex, "Failed to restore layout, reason is: {ex}", ex);
            }
        }

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