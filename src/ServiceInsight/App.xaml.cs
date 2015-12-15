namespace ServiceInsight
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using Anotar.Serilog;
    using DevExpress.Xpf.Core;
    using Framework.Logging;
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
            LoggingConfig.SetupLogging();
            if (!Debugger.IsAttached)
                Framework.ExceptionHandler.Attach();

            Highlighting.Resources.RegisterHighlightings();

            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            LogTo.Information("Starting the application...");
            DXSplashScreen.Show(o => AboutView.AsSplashScreen(), null, null, null);
            base.OnStartup(e);
            LogTo.Information("Application startup finished.");
        }

        public void ShutdownImmediately()
        {
            Environment.Exit(0);
        }
    }
}