namespace ServiceInsight
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using Akavache;
    using Anotar.Serilog;
    using DevExpress.Xpf.Core;
    using Framework;
    using Framework.Logging;
    using Pirac;
    using Shell;

    public interface IAppCommands
    {
        void ShutdownImmediately();
    }

    public class AppCommandsWrapper : IAppCommands
    {
        IAppCommands current;

        public AppCommandsWrapper()
            : this((IAppCommands)Application.Current)
        {
        }

        public AppCommandsWrapper(IAppCommands app)
        {
            current = app;
        }

        public void ShutdownImmediately()
        {
            if (current != null)
            {
                current.ShutdownImmediately();
            }
        }
    }

    public partial class App : IAppCommands
    {
        public App()
        {
            BlobCache.ApplicationName = "ServiceInsight";
            LoggingConfig.SetupLogging();
            if (!Debugger.IsAttached)
            {
                Framework.ExceptionHandler.Attach();
            }

            Highlighting.Resources.RegisterHighlightings();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            PiracRunner.SetContext(new PiracContext { Container = new AutofacContainer(), Logger = s => new SerilogLogger(s) });
            LogTo.Information("Starting the application...");
            DXSplashScreen.Show(o => AboutView.AsSplashScreen(), null, null, null);
            base.OnStartup(e);
            PiracRunner.Start<ShellViewModel>();
            LogTo.Information("Application startup finished.");
        }

        public void ShutdownImmediately()
        {
            Environment.Exit(0);
        }
    }
}