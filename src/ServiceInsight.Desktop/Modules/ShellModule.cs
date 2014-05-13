namespace Particular.ServiceInsight.Desktop.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using Autofac;
    using Core.UI.ScreenManager;
    using ExceptionHandler;
    using ExceptionHandler.Settings;
    using ExceptionHandler.Wpf;
    using Explorer;
    using Explorer.EndpointExplorer;
    using MessageFlow;
    using Options;
    using Search;
    using Shell;
    using Startup;

    public class ShellModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly)
                   .Where(t => t.IsViewOrViewModel() && !ExemptTypes.Contains(t))
                   .AsImplementedInterfaces()
                   .AsSelf()
                   .SingleInstance();

            builder.RegisterSource(new SettingsSource());
            builder.RegisterInstance(new AppCommandsWrapper()).As<IAppCommands>();
            builder.RegisterType<LicenseRegistrationView>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ServiceControlConnectionView>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<EndpointExplorerView>().As<IExplorerView>().InstancePerDependency();
            builder.RegisterType<EndpointExplorerViewModel>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ShellViewModel>().As<IShellViewModel>().SingleInstance().PropertiesAutowired();
            builder.RegisterType<ShellView>().As<IShellView>().SingleInstance().PropertiesAutowired();
            builder.RegisterType<SearchBarView>().SingleInstance();
            builder.RegisterType<StatusBarManager>().SingleInstance();
            builder.RegisterType<AboutView>().InstancePerDependency().PropertiesAutowired();
            builder.RegisterType<ScreenFactory>().SingleInstance();
            builder.RegisterType<DefaultExceptionHandler>().As<IExceptionHandler>().SingleInstance();
            builder.RegisterType<ExceptionViewModel>().As<IExceptionViewModel>().InstancePerDependency();
            builder.RegisterType<OptionsView>().As<IOptionsView>().InstancePerDependency();
            builder.RegisterType<WpfClipboard>().As<IClipboard>().SingleInstance();
            builder.RegisterType<SimpleSettingsReader>().As<ISettingsReader>().WithParameter(TypedParameter.From(ConfigurationManager.AppSettings));
            builder.RegisterType<ExceptionDetailView>().AsImplementedInterfaces().InstancePerDependency();
        }

        protected static IEnumerable<Type> ExemptTypes
        {
            get
            {
                yield return typeof (LicenseRegistrationView);
                yield return typeof (ServiceControlConnectionView);
                yield return typeof (OptionsView);
                yield return typeof (ShellView);
                yield return typeof (ExceptionView);
                yield return typeof (AboutView);
                yield return typeof (ExceptionDetailView);
            }
        }
    }
}