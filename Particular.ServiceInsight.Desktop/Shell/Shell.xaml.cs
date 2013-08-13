﻿using System;
using System.IO;
using System.Windows;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Serialization;
using Particular.ServiceInsight.Desktop.Core.Settings;
using Particular.ServiceInsight.Desktop.ExtensionMethods;
using Particular.ServiceInsight.Desktop.Settings;
using log4net;

namespace Particular.ServiceInsight.Desktop.Shell
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
