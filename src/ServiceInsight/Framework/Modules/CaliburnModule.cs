namespace ServiceInsight.Framework.Modules
{
    using Autofac;
    using Autofac.Core;
    using Caliburn.Micro;
    using ServiceInsight.Framework.UI.ScreenManager;

    public class CaliburnModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WindowManagerEx>().As<IWindowManager>().As<IWindowManagerEx>().InstancePerLifetimeScope();
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().InstancePerLifetimeScope();
        }

        protected override void AttachToComponentRegistration(IComponentRegistry registry, IComponentRegistration registration)
        {
            registration.Activated += OnComponentActivated;
        }

        static void OnComponentActivated(object sender, ActivatedEventArgs<object> e)
        {
            var handler = e.Instance as IHandle;
            if (handler != null)
            {
                e.Context.Resolve<IEventAggregator>().Subscribe(handler);
            }
        }
    }
}