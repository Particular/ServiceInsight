using Autofac;
using Caliburn.Core.Logging;
using NServiceBus.Profiler.Desktop.Logging;

namespace NServiceBus.Profiler.Desktop.Modules
{
    public class LoggerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Log4NetLogger>().As<ILog>();
        }
    }
}