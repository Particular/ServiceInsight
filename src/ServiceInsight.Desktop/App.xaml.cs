using System;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Xpf.Core;
using log4net;
using NServiceBus.Profiler.Desktop.Shell;
using NServiceBus.Profiler.Desktop.Startup;

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
        private static readonly ILog Logger = LogManager.GetLogger("Application");

        public App()
        {
            LoggingConfig.SetupLog4net();
            AppDomain.CurrentDomain.UnhandledException += (s, e) => OnUnhandledException(e);
            WireTaskExceptionHandler();
            InitializeComponent();
        }

        private void WireTaskExceptionHandler()
        {
            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                e.SetObserved();
                LogException(e.Exception);
            };
        }


        private static void OnUnhandledException(UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if (exception != null)
            {
                LogException(exception);
            }
        }

        public static void LogException(Exception ex)
        {
            var baseError = ex.GetBaseException();
            var message = string.Format("An unhandled exception occurred. Error message is {0}.", baseError.Message);

            Logger.Error(message, ex);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Logger.Info("Starting the application...");
            DXSplashScreen.Show(o => AboutView.AsSplashScreen(), null, null, null);
            base.OnStartup(e);
            Logger.Info("Application startup finished.");
        }

        public void ShutdownImmediately()
        {
            Environment.Exit(0);
        }
    }
}
