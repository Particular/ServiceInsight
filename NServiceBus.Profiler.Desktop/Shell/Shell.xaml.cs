using System;
using System.Collections.Generic;
using System.IO;
using Caliburn.Core.Logging;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.NavBar;
using NServiceBus.Profiler.Common.Settings;
using NServiceBus.Profiler.Core.Settings;
using NServiceBus.Profiler.Desktop.MessageHeaders;
using NServiceBus.Profiler.Common.ExtensionMethods;
using Xceed.Wpf.Toolkit.PropertyGrid;

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

        public void SaveLayout(ISettingsProvider settingProvider)
        {
            var layoutSetting = new ShellLayoutSettings();

            layoutSetting.DockLayout = GetLayout(DockManager);
            layoutSetting.MenuLayout = GetLayout(BarManager);

            settingProvider.SaveSettings(layoutSetting);
        }

        public void RestoreLayout(ISettingsProvider settingsProvider)
        {
            var layoutSetting = settingsProvider.GetSettings<ShellLayoutSettings>(true);

            SetLayout(DockManager, layoutSetting.DockLayout.GetAsStream());
            SetLayout(BarManager, layoutSetting.MenuLayout.GetAsStream());
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
                //TODO: Logger not injected?
                //Logger.Info("Failed to save the layout, reason is: " + ex);
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
                //Logger.Info("Failed to restore layout, reason is: " + ex);
            }
        }
    }
}
