using Autofac;
using Autofac.Core;
using Caliburn.PresentationFramework.ApplicationModel;

namespace NServiceBus.Profiler.Desktop.Modules
{
    public class EventAggregationAutoSubscriptionModule : Module
    {
        protected override void AttachToComponentRegistration(IComponentRegistry registry, IComponentRegistration registration)
        {
            registration.Activated += OnComponentActivated;
        }

        static void OnComponentActivated(object sender, ActivatedEventArgs<object> e)
        { 
            var handler = e.Instance as IHandle;
            if (handler != null)
                e.Context.Resolve<IEventAggregator>().Subscribe(handler);
        }
    }
}