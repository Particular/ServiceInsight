using System;
using System.Windows;
using DevExpress.Xpf.Core;
using log4net;
using NServiceBus.Profiler.Desktop.Shell;

namespace NServiceBus.Profiler.Desktop
{
    public interface IAppCommands
    {
        void ShutdownImmediately();
    }

    public class AppCommandsWrapper : IAppCommands
    {
        private readonly IAppCommands _current;

        public AppCommandsWrapper()
            : this((IAppCommands)Application.Current)
        {
        }

        public AppCommandsWrapper(IAppCommands app)
        {
            _current = app;
        }

        public void ShutdownImmediately()
        {
            if (_current != null)
            {
                _current.ShutdownImmediately();
            }
        }
    }

    public partial class App : IAppCommands
    {
        private readonly ILog _logger = LogManager.GetLogger("ApplicationStartup");

        public App()
        {
            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _logger.Info("Starting the application...");
            DXSplashScreen.Show(o => AboutView.AsSplashScreen(), null, null, null);
            base.OnStartup(e);
            _logger.Info("Application startup finished.");
        }

        public void ShutdownImmediately()
        {
            Environment.Exit(0);
        }
    }
}
