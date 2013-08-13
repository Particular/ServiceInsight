using System.Collections.Generic;
using System.Xml;
using Autofac;
using Particular.ServiceInsight.Desktop.Core;
using Particular.ServiceInsight.Desktop.Core.Licensing;
using Particular.ServiceInsight.Desktop.Core.MessageDecoders;
using Particular.ServiceInsight.Desktop.Management;
using Particular.ServiceInsight.Desktop.Models;

namespace Particular.ServiceInsight.Desktop.Modules
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<QueueManager>().As<IQueueManager>().SingleInstance();
            builder.RegisterType<AsyncQueueManager>().As<IQueueManagerAsync>().SingleInstance();
            builder.RegisterType<MSMQueueOperations>().As<IQueueOperations>().As<IQueueOperationsAsync>().SingleInstance();
            builder.RegisterType<StringContentDecoder>().As<IContentDecoder<string>>();
            builder.RegisterType<XmlContentDecoder>().As<IContentDecoder<XmlDocument>>();
            builder.RegisterType<HeaderContentDecoder>().As<IContentDecoder<IList<HeaderInfo>>>();
            builder.RegisterType<DefaultMapper>().As<IMapper>().SingleInstance();
            builder.RegisterType<NetworkOperations>().As<INetworkOperations>().SingleInstance();
            builder.RegisterType<AppLicenseManager>().As<ILicenseManager>().SingleInstance();
            builder.RegisterType<CryptographyService>().As<ICryptoService>().SingleInstance();
            builder.RegisterType<ManagementConnectionProvider>().As<IManagementConnectionProvider>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultManagementService>().As<IManagementService>().InstancePerLifetimeScope();
        }
    }
}