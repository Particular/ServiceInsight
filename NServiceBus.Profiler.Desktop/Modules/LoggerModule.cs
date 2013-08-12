using Autofac;
using Caliburn.Core.Logging;
using Particular.ServiceInsight.Desktop.Logging;

namespace Particular.ServiceInsight.Desktop.Modules
{
    public class LoggerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Log4NetLogger>().As<ILog>();
        }
    }
}