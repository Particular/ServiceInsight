namespace Particular.ServiceInsight.Desktop
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using DevExpress.Xpf.Core;
    using Framework.Logging;
    using Serilog;
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
        static ILogger Logger = Log.ForContext<App>();

        public App()
        {
            LoggingConfig.SetupLogging();
            AppDomain.CurrentDomain.UnhandledException += (s, e) => OnUnhandledException(e);
            WireTaskExceptionHandler();
            InitializeComponent();
        }

        void WireTaskExceptionHandler()
        {
            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                e.SetObserved();
                LogException(e.Exception);
            };
        }

        static void OnUnhandledException(UnhandledExceptionEventArgs e)
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
            Logger.Information("Starting the application...");
            DXSplashScreen.Show(o => AboutView.AsSplashScreen(), null, null, null);
            base.OnStartup(e);
            Logger.Information("Application startup finished.");
        }

        public void ShutdownImmediately()
        {
            Environment.Exit(0);
        }
    }
}