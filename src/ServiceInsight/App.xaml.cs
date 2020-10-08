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
        private bool createdNew;

        public App()
        {
            var _ = new Mutex(true, "ServiceInsight", out createdNew);

            if (createdNew)
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
            if (!createdNew)
            {
                App.Current.Shutdown();
                FocusWindow.ShowWindow();
                return;
            }

            LogTo.Information("Starting the application...");

            var args = new CommandLineArgParser();

            if (!args.ParsedOptions.SilentStartup)
            {
                DXSplashScreen.Show(o => AboutView.AsSplashScreen(), null, null, null);
            }

            ApplicationConfiguration.Initialize();
            AppBootstrapper.CreateContainer();
            base.OnStartup(e);
            LogTo.Information("Application startup finished.");
        }
    }
}