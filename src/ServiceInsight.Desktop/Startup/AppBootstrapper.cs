using System.Windows.Threading;

namespace Particular.ServiceInsight.Desktop.Startup
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Markup;
    using Autofac;
    using Caliburn.Micro;
    using DevExpress.Xpf.Bars;
    using ExceptionHandler;
    using Shell;
    using IContainer = Autofac.IContainer;

    public class AppBootstrapper : Bootstrapper<ShellViewModel>
    {
        protected IContainer container;

        protected override void Configure()
        {
            CreateContainer();
            ExtendConventions();
            ApplyBindingCulture();
        }

        private void CreateContainer()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());
            container = containerBuilder.Build();

            // We reregister the container within itself.
            // This is bad and we should feel bad about it.
            containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterInstance<IContainer>(container).SingleInstance();
            containerBuilder.Update(container);
        }

        void ApplyBindingCulture()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        void ExtendConventions()
        {
            ConventionManager.AddElementConvention<BarButtonItem>(BarButtonItem.IsVisibleProperty, "DataContext", "ItemClick");
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (!Debugger.IsAttached)
                e.Handled = TryHandleException(e.Exception);
        }

        protected virtual bool TryHandleException(Exception exception)
        {
            try
            {
                var handler = container.Resolve<IExceptionHandler>();
                handler.Handle(exception);
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.Resolve(typeof(IEnumerable<>).MakeGenericType(new Type[] { service })) as IEnumerable<object>;
        }

        protected override object GetInstance(Type service, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                object result;
                if (container.TryResolve(service, out result))
                {
                    return result;
                }
            }
            else
            {
                object result;
                if (container.TryResolveNamed(key, service, out result))
                {
                    return result;
                }
            }
            throw new Exception(string.Format("Could not locate any instances of contract {0}.", key ?? service.Name));
        }

        protected override void BuildUp(object instance)
        {
            container.InjectProperties(instance);
        }
    }
}