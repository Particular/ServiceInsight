using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Autofac;
using ExceptionHandler;
using ExceptionHandler.Settings;
using ExceptionHandler.Wpf;
using Particular.ServiceInsight.Desktop.Explorer;
using Particular.ServiceInsight.Desktop.Explorer.EndpointExplorer;
using Particular.ServiceInsight.Desktop.Explorer.QueueExplorer;
using Particular.ServiceInsight.Desktop.Options;
using Particular.ServiceInsight.Desktop.ScreenManager;
using Particular.ServiceInsight.Desktop.Search;
using Particular.ServiceInsight.Desktop.Shell;
using Particular.ServiceInsight.Desktop.Startup;

namespace Particular.ServiceInsight.Desktop.Modules
{
    public class ShellModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly)
                   .Where(t => t.IsViewOrViewModel() && !ExcemptTypes.Contains(t))
                   .AsImplementedInterfaces()
                   .AsSelf()
                   .SingleInstance();

            builder.RegisterSource(new SettingsSource());
            builder.RegisterInstance(new AppCommandsWrapper()).As<IAppCommands>();
            builder.RegisterType<QueueCreationView>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ConnectToMachineView>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<LicenseRegistrationView>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ManagementConnectionView>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<EndpointExplorerView>().As<IExplorerView>().InstancePerDependency();
            builder.RegisterType<QueueExplorerView>().As<IExplorerView>().InstancePerDependency();
            builder.RegisterType<EndpointExplorerViewModel>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<QueueExplorerViewModel>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ShellViewModel>().As<IShellViewModel>().SingleInstance().PropertiesAutowired();
            builder.RegisterType<ShellView>().As<IShellView>().SingleInstance().PropertiesAutowired();
            builder.RegisterType<SearchBar>().As<ISearchBarView>().SingleInstance();
            builder.RegisterType<MenuManager>().As<IMenuManager>().SingleInstance();
            builder.RegisterType<StatusBarManager>().As<IStatusBarManager>().SingleInstance();
            builder.RegisterType<ScreenFactory>().As<IScreenFactory>().SingleInstance();
            builder.RegisterType<DefaultExceptionHandler>().As<IExceptionHandler>().SingleInstance();
            builder.RegisterType<ExceptionViewModel>().As<IExceptionViewModel>().InstancePerDependency();
            builder.RegisterType<OptionsView>().As<IOptionsView>().InstancePerDependency();
            builder.RegisterType<ExceptionView>().As<ExceptionView>().InstancePerDependency();
            builder.RegisterType<WpfClipboard>().As<IClipboard>().SingleInstance();
//            builder.RegisterType<BitBucketIssueTracker>().As<IIssueTracker>();
//            builder.RegisterType<BitBucketIssueRelayApi>();
            builder.RegisterType<SimpleSettingsReader>().As<ISettingsReader>().WithParameter(TypedParameter.From(ConfigurationManager.AppSettings));
        }

        protected static IEnumerable<Type> ExcemptTypes
        {
            get
            {
                yield return typeof (QueueCreationView);
                yield return typeof (ConnectToMachineView);
                yield return typeof (LicenseRegistrationView);
                yield return typeof (ManagementConnectionView);
                yield return typeof (OptionsView);
                yield return typeof (ShellView);
                yield return typeof (ExceptionView);
            }
        }
    }
}