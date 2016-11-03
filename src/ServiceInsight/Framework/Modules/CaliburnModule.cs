namespace ServiceInsight.Framework.Modules
{
    using Autofac;
    using Caliburn.Micro;
    using ServiceInsight.Framework.UI.ScreenManager;

    public class CaliburnModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<WindowManagerEx>().InstancePerLifetimeScope();
            //builder.RegisterType<DefaultEventAggregator>().As<IEventAggregator>().InstancePerLifetimeScope();
            //builder.RegisterType<DefaultDispatcher>().As<IDispatcher>().InstancePerLifetimeScope();

            builder.RegisterType<WindowManagerEx>().As<IWindowManager>().As<IWindowManagerEx>().InstancePerLifetimeScope();
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().InstancePerLifetimeScope();

            //  register view models
            //builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
            //  .Where(type => type.Name.EndsWith("ViewModel"))
            //  .AsSelf()
            //  .InstancePerDependency();

            //  register views
            //builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
            //  .Where(type => type.Name.EndsWith("View"))
            //  .AsSelf()
            //  .InstancePerDependency();
        }
    }
}