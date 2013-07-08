using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Threading;
using Autofac;
using Caliburn.Core.InversionOfControl;
using Caliburn.PresentationFramework.ApplicationModel;
using ExceptionHandler;
using log4net;
using NServiceBus.Profiler.Desktop.Shell;
using IContainer = Autofac.IContainer;

namespace NServiceBus.Profiler.Desktop.Startup
{
    public class AppBootstrapper : Bootstrapper<IShellViewModel>
    {
        private ILog _logger = LogManager.GetLogger(typeof(AppBootstrapper));
        private IContainer _container;
        
        public AppBootstrapper()
        {
            ConfigLogger();
            WireTaskExceptionHandler();
        }

        private void WireTaskExceptionHandler()
        {
            TaskScheduler.UnobservedTaskException += (s, e) => TryDisplayUnhandledException(e.Exception);
        }

        public IContainer GetContainer()
        {
            return _container;
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            TryDisplayUnhandledException(e.Exception);
        }

        protected override IServiceLocator CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());
            _container = builder.Build();

            return new AutofacAdapter(_container);
        }

        private void ConfigLogger()
        {
            new LoggingConfig().SetupLog4net();
            //_logger = new LoggingConfig().GetLogger();
            //LogManager.Initialize(type => _logger);
        }

        protected virtual void TryDisplayUnhandledException(Exception exception)
        {
            try
            {
                _logger.Error(exception);
                var handler = _container.Resolve<IExceptionHandler>();
                handler.Handle(exception);
            }
            catch(Exception ex)
            {
                _logger.Error("Failed to display exception dialog", ex);
                Environment.Exit(-1);
            }
        }
    }
}
