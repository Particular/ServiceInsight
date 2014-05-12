namespace Particular.ServiceInsight.Desktop.Startup
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Threading;
    using Autofac;
    using Caliburn.Core.InversionOfControl;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Conventions;
    using DevExpress.Xpf.Bars;
    using ExceptionHandler;
    using Shell;
    using IContainer = Autofac.IContainer;

    public class AppBootstrapper : Bootstrapper<IShellViewModel>
    {
        IContainer container;
        
        protected override void PrepareApplication()
        {
            base.PrepareApplication();
            ExtendConventions();
            ApplyBindingCulture();
        }

        void ApplyBindingCulture()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        void ExtendConventions()
        {
            var convention = Container.GetInstance<IConventionManager>();
            convention.AddElementConvention(new DefaultElementConvention<BarButtonItem>("ItemClick", BarButtonItem.IsVisibleProperty, (item, o) => item.DataContext = o, item => item.DataContext));
        }

        public IContainer GetContainer()
        {
            return container;
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = TryHandleException(e.Exception);
        }

        protected override IServiceLocator CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());
            container = builder.Build();

            return new AutofacAdapter(container);
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
    }
}
