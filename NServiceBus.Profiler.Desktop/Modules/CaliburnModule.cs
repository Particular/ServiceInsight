using Autofac;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Invocation;
using NServiceBus.Profiler.Desktop.ScreenManager;

namespace NServiceBus.Profiler.Desktop.Modules
{
    public class CaliburnModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WindowManagerEx>().As<IWindowManager>().As<IWindowManagerEx>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultEventAggregator>().As<IEventAggregator>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultDispatcher>().As<IDispatcher>().InstancePerLifetimeScope();
        }
    }
}