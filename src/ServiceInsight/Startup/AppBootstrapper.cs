namespace ServiceInsight.Startup
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Markup;
    using Autofac;
    using Caliburn.Micro;
    using DevExpress.Xpf.Bars;
    using DevExpress.Xpf.Core;
    using ExtensionMethods;
    using Framework;
    using Framework.Logging;
    using ServiceInsight.Framework.Settings;
    using Shell;
    using IContainer = Autofac.IContainer;

    public class AppBootstrapper : BootstrapperBase
    {
        protected IContainer container;

        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            ApplicationConfiguration.Initialize();

            CreateContainer();
            ExtendConventions();
            ApplyBindingCulture();
            SetupUIAutomation();

            LoggingConfig.SetupCaliburnMicroLogging();

            var newHandler = container.Resolve<AppExceptionHandler>(); //TODO: Yuck! Fix the ExceptionHandler dependencies to get around this
            var defaultHandler = ExceptionHandler.HandleException;
            ExceptionHandler.HandleException = ex => newHandler.Handle(ex, defaultHandler);
        }

        [Conditional("DEBUG")]
        static void SetupUIAutomation()
        {
            ClearAutomationEventsHelper.IsEnabled = false;
        }

        void CreateContainer()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterAssemblyModules(typeof(AppBootstrapper).Assembly);
            containerBuilder.RegisterExternalModules();
            container = containerBuilder.Build();

            // We reregister the container within itself.
            // This is bad and we should feel bad about it.
            containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterInstance(container).SingleInstance();
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

        protected override void PrepareApplication()
        {
            Application.Startup += OnStartup;
            Application.Exit += OnExit;
        }

        protected override IEnumerable<object> GetAllInstances(Type service) => container.Resolve(typeof(IEnumerable<>).MakeGenericType(new[] { service })) as IEnumerable<object>;

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

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}