namespace Particular.ServiceInsight.Desktop.Modules
{
    using Autofac;
    using Caliburn.Micro;
    using Logging;

    public class LoggerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CaliburnMicroLogAdapter>().As<ILog>();
        }
    }
}