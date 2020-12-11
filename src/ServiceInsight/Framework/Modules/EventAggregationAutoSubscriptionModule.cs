using Autofac.Core.Registration;
using Autofac.Core.Resolving.Pipeline;

namespace ServiceInsight.Framework.Modules
{
    using Autofac;
    using Autofac.Core;
    using Caliburn.Micro;

    public class EventAggregationAutoSubscriptionModule : Module
    {
        protected override void AttachToComponentRegistration(
            IComponentRegistryBuilder componentRegistry, 
            IComponentRegistration registration)
        {
            registration.PipelineBuilding += (sender, pipeline) =>
            {
                pipeline.Use(PipelinePhase.Activation, MiddlewareInsertionMode.EndOfPhase, (c, next) =>
                {
                    next(c);

                    if (c.Instance is IHandle handler)
                    {
                        c.Resolve<IEventAggregator>().Subscribe(handler);
                    }
                });
            };
        } 
    }
}