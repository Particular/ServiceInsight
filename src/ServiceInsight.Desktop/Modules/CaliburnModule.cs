namespace Particular.ServiceInsight.Desktop.Modules
{
    using Autofac;
    using Caliburn.Micro;
    using Core.UI.ScreenManager;

    public class CaliburnModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WindowManagerEx>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultEventAggregator>().As<IEventAggregator>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultDispatcher>().As<IDispatcher>().InstancePerLifetimeScope();
        }
    }
}