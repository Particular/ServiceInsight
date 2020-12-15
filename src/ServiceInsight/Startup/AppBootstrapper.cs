using System.IO;
using System.Linq;
using System.Reflection;
using ServiceInsight.AssemblyScanning;
using ServiceInsight.ExtensionMethods;

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
    using Framework;
    using Framework.Logging;
    using Shell;

    public class AppBootstrapper : BootstrapperBase
    {
        Autofac.IContainer container;
        IList<Assembly> assemblies;

        public AppBootstrapper()
        {
            assemblies = new List<Assembly>
            {
                typeof(AppBootstrapper).Assembly
            };
            
            if (!InDesignMode())
            {
                Initialize();
            }
        }

        protected override void Configure()
        {
            CreateContainer();
            ExtendConventions();
            ApplyBindingCulture();
            SetupUIAutomation();

            LoggingConfig.SetupCaliburnMicroLogging();

            var newHandler = container.Resolve<AppExceptionHandler>();
            var defaultHandler = ExceptionHandler.HandleException;
            ExceptionHandler.HandleException = ex => newHandler.Handle(ex, defaultHandler);
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

        void CreateContainer()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterAssemblyModules(assemblies.ToArray());
            container = containerBuilder.Build();
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            var applicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var serviceInsightApplicationData = Path.Combine(applicationData, "Particular Software", "ServiceInsight", "MessageViewers");
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var serviceInsightProgramData = Path.Combine(programData, "Particular", "ServiceInsight","MessageViewers");

            var scanner = new AssemblyScanner(serviceInsightApplicationData, serviceInsightProgramData);
            assemblies.AddRange(scanner.Scan());

            return assemblies;
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