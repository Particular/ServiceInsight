namespace ServiceInsight
{
    using System.Diagnostics;
    using System.Threading;
    using System.Windows;
    using Anotar.Serilog;
    using DevExpress.Xpf.Core;
    using Framework.Logging;
    using ServiceInsight.Framework.Settings;
    using ServiceInsight.Startup;
    using Shell;

    public partial class App
    {
        bool createNew;
        Mutex mutex;
        
        public App()
        {
            mutex = new Mutex(true, "ServiceInsight", out createNew);
            if (createNew)
            {
                LoggingConfig.SetupLogging();
                if (!Debugger.IsAttached)
                {
                    Framework.ExceptionHandler.Attach();
                }

                Highlighting.Resources.RegisterHighlightings();
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var args = new CommandLineArgParser();
            
            if (!createNew)
            {
                args.SendToOtherInstance();
                App.Current.Shutdown();
                FocusWindow.ShowWindow();
                return;
            }

            LogTo.Information("Starting the application...");

            if (!args.ParsedOptions.SilentStartup)
            {
                DXSplashScreen.Show(o => AboutView.AsSplashScreen(), null, null, null);
            }

            ApplicationConfiguration.Initialize();
            base.OnStartup(e);
            LogTo.Information("Application startup finished.");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            mutex.Dispose();
        }
    }
}