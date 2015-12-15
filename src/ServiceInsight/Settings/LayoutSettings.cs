namespace ServiceInsight.Settings
{
    using System.ComponentModel;
    using System.Configuration;
    using System.Windows;

    [SettingsProvider("IsolatedStore")]
    public class ShellLayoutSettings
    {
        [DefaultValue(null)]
        public string DockLayout { get; set; }

        [DefaultValue(null)]
        public string MenuLayout { get; set; }

        [DefaultValue(null)]
        public string LayoutVersion { get; set; }

        [DefaultValue(false)]
        public bool ResetLayout { get; set; }

        [DefaultValue(0)]
        public double MainWindowTop { get; set; }

        [DefaultValue(0)]
        public double MainWindowLeft { get; set; }

        [DefaultValue(400)]
        public double MainWindowHeight { get; set; }

        [DefaultValue(600)]
        public double MainWindowWidth { get; set; }

        [DefaultValue(typeof(WindowState), "Maximized")]
        public WindowState MainWindowState { get; set; }
    }
}