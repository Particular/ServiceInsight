using System;
using System.IO;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Serialization;
using log4net;
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
        private readonly ILog _logger = LogManager.GetLogger(typeof (IShellView));

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

        public void SaveLayout(ISettingsProvider settingProvider)
        {
            var layoutSetting = settingProvider.GetSettings<ShellLayoutSettings>();

            if (!layoutSetting.ResetLayout)
            {
                layoutSetting.LayoutVersion = GetCurrentLayoutVersion();
                layoutSetting.DockLayout = GetLayout(DockManager);
                layoutSetting.MenuLayout = GetLayout(BarManager);
            }
            else
            {
                layoutSetting.ResetLayout = false;
            }

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

        public void ResetLayout(ISettingsProvider settingsProvider)
        {
            var layoutSettings = settingsProvider.GetSettings<ShellLayoutSettings>(true);

            layoutSettings.ResetLayout = true;
            layoutSettings.MenuLayout = null;
            layoutSettings.DockLayout = null;
            
            settingsProvider.SaveSettings(layoutSettings);
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
                _logger.Info("Failed to save the layout, reason is: " + ex);
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
                _logger.Info("Failed to restore layout, reason is: " + ex);
            }
        }
    }
}
