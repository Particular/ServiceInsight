namespace ServiceInsight.Framework.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;
    using Explorer.EndpointExplorer;
    using ExtensionMethods;
    using MessageFlow;
    using Options;
    using Search;
    using ServiceInsight.MessageList;
    using Shell;

    public class ShellModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DefaultClipboard>().As<IClipboard>().InstancePerDependency();

            builder.RegisterAssemblyTypes(ThisAssembly)
                   .Where(t => t.IsViewOrViewModel() && !ExemptTypes.Contains(t))
                   .AsImplementedInterfaces()
                   .AsSelf()
                   .SingleInstance();

            builder.RegisterAssemblyTypes(ThisAssembly)
                .Where(t => t.IsAttachment())
                .AsSelf()
                .InstancePerDependency();

            builder.RegisterInstance(new AppCommandsWrapper()).As<IAppCommands>();
            builder.RegisterType<LicenseRegistrationView>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ServiceControlConnectionView>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<EndpointExplorerView>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<EndpointExplorerViewModel>().SingleInstance();
            builder.RegisterType<ShellViewModel>().SingleInstance().PropertiesAutowired();
            builder.RegisterType<MessageSelectionContext>().SingleInstance();
            builder.RegisterType<ShellView>().As<IShellView>().SingleInstance().PropertiesAutowired();
            builder.RegisterType<SearchBarView>().SingleInstance();
            builder.RegisterType<StatusBarManager>().SingleInstance();
            builder.RegisterType<AboutView>().InstancePerDependency().PropertiesAutowired();
            builder.RegisterType<AppExceptionHandler>().SingleInstance();
            builder.RegisterType<OptionsView>().As<IOptionsView>().InstancePerDependency();
            builder.RegisterType<ExceptionDetailView>().AsImplementedInterfaces().AsSelf().InstancePerDependency();
        }

        protected static IEnumerable<Type> ExemptTypes
        {
            get
            {
                yield return typeof(LicenseRegistrationView);
                yield return typeof(ServiceControlConnectionView);
                yield return typeof(MessageSelectionContext);
                yield return typeof(OptionsView);
                yield return typeof(ShellView);
                yield return typeof(AboutView);
                yield return typeof(ExceptionDetailView);
            }
        }
    }
}