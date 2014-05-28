namespace Particular.ServiceInsight.Desktop
{
    using System;
    using System.Threading.Tasks;
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
            Framework.ExceptionHandler.Attach();
            //AppDomain.CurrentDomain.UnhandledException += (s, e) => OnUnhandledException(e);
            //WireTaskExceptionHandler();
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

            LogTo.Error(ex, "An unhandled exception occurred. Error message is {Message}.", baseError.Message);
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