using System;
using System.Windows;
using DevExpress.Xpf;
using DevExpress.Xpf.Core;
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
        public App()
        {
            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            DXSplashScreen.Show(o => AboutView.AsSplashScreen(), null, null, null);
            base.OnStartup(e);
        }

        public void ShutdownImmediately()
        {
            Environment.Exit(0);
        }
    }
}
