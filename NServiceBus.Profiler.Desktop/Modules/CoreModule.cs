using System.Xml;
using Autofac;
using NServiceBus.Profiler.Core;
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
            builder.RegisterType<StringMessageDecoder>().As<IMessageDecoder<string>>();
            builder.RegisterType<XmlMessageDecoder>().As<IMessageDecoder<XmlDocument>>();
            builder.RegisterType<DefaultMapper>().As<IMapper>().SingleInstance();
            builder.RegisterType<DefaultManagementService>().As<IManagementService>().SingleInstance();
            builder.RegisterType<NetworkOperations>().As<INetworkOperations>().SingleInstance();
        }
    }
}