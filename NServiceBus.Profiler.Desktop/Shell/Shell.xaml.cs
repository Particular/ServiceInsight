using System;
using System.IO;
using Caliburn.Core.Logging;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Serialization;
using NServiceBus.Profiler.Common.Settings;
using NServiceBus.Profiler.Core.Settings;
using NServiceBus.Profiler.Common.ExtensionMethods;

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
            BarManager.CheckBarItemNames = false;
        }

        public void ChangeTheme(string name)
        {
            ThemeManager.ApplicationThemeName = name;
        }

        public BarManager GetMenuManager()
        { 
            return BarManager;
        }

        public ILog Logger { get; set; }

        public void SaveLayout(ISettingsProvider settingProvider)
        {
            var layoutSetting = new ShellLayoutSettings();

            layoutSetting.LayoutVersion = GetCurrentLayoutVersion();
            layoutSetting.DockLayout = GetLayout(DockManager);
            layoutSetting.MenuLayout = GetLayout(BarManager);

            settingProvider.SaveSettings(layoutSetting);
        }

        public void RestoreLayout(ISettingsProvider settingsProvider)
        {
            var layoutSetting = settingsProvider.GetSettings<ShellLayoutSettings>(true);
            var currentLayoutVersion = GetCurrentLayoutVersion();

            if (layoutSetting.LayoutVersion == currentLayoutVersion)
            {
                SetLayout(DockManager, layoutSetting.DockLayout.GetAsStream());
                SetLayout(BarManager, layoutSetting.MenuLayout.GetAsStream());
            }
        }

        private string GetCurrentLayoutVersion()
        {
            return DXSerializer.GetLayoutVersion(BarManager);
        }

        private string GetLayout(dynamic control) //Lack of common interface :(
        {
            try
            {
                var ms = new MemoryStream();
                control.SaveLayoutToStream(ms);
                return ms.GetAsString();
            }
            catch (Exception ex)
            {
                Logger.Info("Failed to save the layout, reason is: " + ex);
                return null;
            }
        }

        private void SetLayout(dynamic control, Stream layout)
        {
            if(layout == null)
                return;

            try
            {
                control.RestoreLayoutFromStream(layout);
            }
            catch(Exception ex)
            {
                Logger.Info("Failed to restore layout, reason is: " + ex);
            }
        }
    }
}
