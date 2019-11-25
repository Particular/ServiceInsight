namespace ServiceInsight
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using Anotar.Serilog;
    using DevExpress.Xpf.Core;
    using Framework.Logging;
    using ServiceInsight.Framework.Settings;
    using Shell;

    public partial class App
    {
        public App()
        {
            LoggingConfig.SetupLogging();
            if (!Debugger.IsAttached)
            {
                Framework.ExceptionHandler.Attach();
            }

            Highlighting.Resources.RegisterHighlightings();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            LogTo.Information("Starting the application...");

            var doNotShowSplashScreen = e.Args.Length > 0 && e.Args.Contains ("--silent", StringComparer.OrdinalIgnoreCase);

            if (!doNotShowSplashScreen)
            {
                DXSplashScreen.Show(o => AboutView.AsSplashScreen(), null, null, null);
            }

            ApplicationConfiguration.Initialize();
            base.OnStartup(e);
            LogTo.Information("Application startup finished.");
        }
    }
}