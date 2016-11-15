namespace ServiceInsight.Framework.Modules
{
    using System.Collections.Generic;
    using System.Xml;
    using Autofac;
    using Models;
    using ServiceControl;
    using ServiceInsight.Framework.Licensing;
    using ServiceInsight.Framework.MessageDecoders;
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
            builder.RegisterType<WorkNotifier>().As<IWorkNotifier>().SingleInstance();
        }
    }
}