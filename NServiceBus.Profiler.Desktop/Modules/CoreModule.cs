using System.Collections.Generic;
using System.Xml;
using Autofac;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core;
using NServiceBus.Profiler.Core.Licensing;
using NServiceBus.Profiler.Core.Management;
using NServiceBus.Profiler.Core.MessageDecoders;

namespace NServiceBus.Profiler.Desktop.Modules
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