namespace Particular.ServiceInsight.Desktop.Framework.Modules
{
    using System.Collections.Generic;
    using System.Xml;
    using Autofac;
    using Models;
    using Particular.ServiceInsight.Desktop.Framework.Licensing;
    using Particular.ServiceInsight.Desktop.Framework.MessageDecoders;
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
            builder.RegisterType<ServiceControlConnectionProvider>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultServiceControl>().As<IServiceControl>().InstancePerLifetimeScope();
            builder.RegisterType<CommandLineArgParser>().SingleInstance().OnActivating(e => e.Instance.Parse());
        }
    }
}