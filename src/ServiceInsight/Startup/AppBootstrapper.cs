namespace ServiceInsight.Startup
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
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
    using Shell;
    using IContainer = Autofac.IContainer;

    public class AppBootstrapper : BootstrapperBase
    {
        public static IContainer Container { get; private set; }

        public AppBootstrapper()
        {
            if (!InDesignMode())
            {
                Initialize();
            }
        }

        protected override void Configure()
        {
            ExtendConventions();
            ApplyBindingCulture();
            SetupUIAutomation();

            LoggingConfig.SetupCaliburnMicroLogging();
        }

        static bool InDesignMode()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject());
        }

        [Conditional("DEBUG")]
        static void SetupUIAutomation()
        {
            ClearAutomationEventsHelper.IsEnabled = false;
        }

        public static void CreateContainer()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterAssemblyModules(typeof(AppBootstrapper).Assembly);
            containerBuilder.RegisterExternalModules();
            Container = containerBuilder.Build();

            // We reregister the container within itself.
            // This is bad and we should feel bad about it.
            containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterInstance(Container).SingleInstance();
            containerBuilder.Update(Container);
        }

        void ApplyBindingCulture()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        void ExtendConventions()
        {
            ConventionManager.AddElementConvention<BarButtonItem>(BarButtonItem.IsVisibleProperty, "DataContext", "ItemClick");
        }

        void ResolveExceptionHandler()
        {
            var newHandler = Container.Resolve<AppExceptionHandler>(); //TODO: Yuck! Fix the ExceptionHandler dependencies to get around this
            var defaultHandler = ExceptionHandler.HandleException;
            ExceptionHandler.HandleException = ex => newHandler.Handle(ex, defaultHandler);
        }

        protected override void PrepareApplication()
        {
            Application.Startup += OnStartup;
            Application.Exit += OnExit;
        }

        protected override IEnumerable<object> GetAllInstances(Type service) => Container.Resolve(typeof(IEnumerable<>).MakeGenericType(new[] { service })) as IEnumerable<object>;

        protected override object GetInstance(Type service, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                object result;
                if (Container.TryResolve(service, out result))
                {
                    return result;
                }
            }
            else
            {
                object result;
                if (Container.TryResolveNamed(key, service, out result))
                {
                    return result;
                }
            }
            throw new Exception(string.Format("Could not locate any instances of contract {0}.", key ?? service.Name));
        }

        protected override void BuildUp(object instance)
        {
            Container.InjectProperties(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            ResolveExceptionHandler();
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}