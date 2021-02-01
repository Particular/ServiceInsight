namespace ServiceInsight.Framework.Modules
{
    using ServiceInsight.MessageViewers.CustomMessageViewer;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;
    using ServiceInsight.Explorer.EndpointExplorer;
    using ServiceInsight.ExtensionMethods;
    using ServiceInsight.MessageFlow;
    using ServiceInsight.Options;
    using ServiceInsight.Search;
    using ServiceInsight.MessageList;
    using ServiceInsight.MessagePayloadViewer;
    using ServiceInsight.Shell;

    public class ShellModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DefaultClipboard>().As<IClipboard>().InstancePerDependency();
            builder.RegisterType<CustomMessageViewerResolver>().As<ICustomMessageViewerResolver>().SingleInstance();

            builder.RegisterAssemblyTypes(ThisAssembly)
                   .Where(t => t.IsViewOrViewModel() && !ExemptTypes.Contains(t))
                   .AsImplementedInterfaces()
                   .AsSelf()
                   .SingleInstance();

            builder.RegisterAssemblyTypes(ThisAssembly)
                .Where(t => t.IsAttachment())
                .AsSelf()
                .InstancePerDependency();

            builder.RegisterType<AppCommands>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<LicenseMessageBoxView>().AsImplementedInterfaces().ExternallyOwned();
            builder.RegisterType<ManageLicenseView>().AsImplementedInterfaces().ExternallyOwned();
            builder.RegisterType<ServiceControlConnectionView>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<EndpointExplorerView>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<EndpointExplorerViewModel>().SingleInstance();
            builder.RegisterType<ShellViewModel>().SingleInstance().PropertiesAutowired();
            builder.RegisterType<MessageSelectionContext>().SingleInstance();
            builder.RegisterType<ShellView>().As<IShellView>().SingleInstance().PropertiesAutowired();
            builder.RegisterType<SearchBarView>().SingleInstance();
            builder.RegisterType<StatusBarManager>().SingleInstance();
            builder.RegisterType<LicenseStatusBar>().SingleInstance();
            builder.RegisterType<AboutView>().InstancePerDependency().PropertiesAutowired();
            builder.RegisterType<AppExceptionHandler>().SingleInstance();
            builder.RegisterType<OptionsView>().As<IOptionsView>().InstancePerDependency();
            builder.RegisterType<ExceptionDetailView>().AsImplementedInterfaces().AsSelf().InstancePerDependency();
            builder.RegisterType<MessagePayloadView>().AsImplementedInterfaces().AsSelf().InstancePerDependency();
            builder.RegisterType<VersionUpdateChecker>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ApplicationVersionService>().As<IApplicationVersionService>().SingleInstance();
        }

        protected static IEnumerable<Type> ExemptTypes
        {
            get
            {
                yield return typeof(LicenseMessageBoxView);
                yield return typeof(ServiceControlConnectionView);
                yield return typeof(MessageSelectionContext);
                yield return typeof(OptionsView);
                yield return typeof(ShellView);
                yield return typeof(AboutView);
                yield return typeof(ExceptionDetailView);
                yield return typeof(ManageLicenseView);
                yield return typeof(MessagePayloadView);
            }
        }
    }
}