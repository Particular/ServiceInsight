using System.Collections.Generic;
using System.Xml;
using Autofac;
using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.Core.Licensing;
using NServiceBus.Profiler.Desktop.Core.MessageDecoders;
using NServiceBus.Profiler.Desktop.Management;
using NServiceBus.Profiler.Desktop.MessageProperties;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.Modules
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<QueueManager>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<AsyncQueueManager>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<MSMQueueOperations>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<StringContentDecoder>().As<IContentDecoder<string>>();
            builder.RegisterType<XmlContentDecoder>().As<IContentDecoder<XmlDocument>>();
            builder.RegisterType<HeaderContentDecoder>().As<IContentDecoder<IList<HeaderInfo>>>();
            builder.RegisterType<DefaultMapper>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<NetworkOperations>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<AppLicenseManager>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<CryptographyService>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ManagementConnectionProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<DefaultManagementService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<HeaderInfoSerializer>().AsImplementedInterfaces();
        }
    }
}