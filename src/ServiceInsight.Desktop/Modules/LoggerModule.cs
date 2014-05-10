namespace Particular.ServiceInsight.Desktop.Modules
{
    using Autofac;
    using Caliburn.Core.Logging;
    using Logging;

    public class LoggerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Log4NetLogger>().As<ILog>();
        }
    }
}