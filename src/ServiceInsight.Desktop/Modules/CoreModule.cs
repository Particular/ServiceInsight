namespace Particular.ServiceInsight.Desktop.Modules
{
    using System.Collections.Generic;
    using System.Xml;
    using Autofac;
    using Core;
    using Core.Licensing;
    using Core.MessageDecoders;
    using MessageProperties;
    using Models;
    using ServiceControl;
    using Startup;

    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StringContentDecoder>().As<IContentDecoder<string>>();
            builder.RegisterType<XmlContentDecoder>().As<IContentDecoder<XmlDocument>>();
            builder.RegisterType<HeaderContentDecoder>().As<IContentDecoder<IList<HeaderInfo>>>();
            builder.RegisterType<NetworkOperations>().SingleInstance();
            builder.RegisterType<AppLicenseManager>().SingleInstance();
            builder.RegisterType<ServiceControlConnectionProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<DefaultServiceControl>().InstancePerLifetimeScope();
            builder.RegisterType<HeaderInfoSerializer>().AsImplementedInterfaces();
            builder.RegisterType<CommandLineArgParser>().AsImplementedInterfaces().SingleInstance().OnActivating(e => e.Instance.Parse());
        }
    }
}