namespace ServiceInsight.Framework.Modules
{
    using Autofac;
    using Caliburn.Micro;
    using ServiceInsight.Framework.UI.ScreenManager;

    public class CaliburnModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WindowManagerEx>().As<Caliburn.Micro.IWindowManager>().As<IWindowManagerEx>().InstancePerLifetimeScope();
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().InstancePerLifetimeScope();
        }
    }
}