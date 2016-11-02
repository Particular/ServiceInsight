namespace ServiceInsight.Framework.Modules
{
    using System.Linq;
    using Autofac;
    using Autofac.Core;
    using Pirac;

    class AutoAttachmentModule : Module
    {
        private IConventionManager conventionManager;

        protected override void AttachToComponentRegistration(
            IComponentRegistry componentRegistry,
            IComponentRegistration registration)
        {
            registration.Activating += Activating;
        }

        void Activating(object sender, ActivatingEventArgs<object> e)
        {
            if (e.Instance is IConventionManager)
            {
                return;
            }

            if (conventionManager == null)
            {
                conventionManager = e.Context.Resolve<IConventionManager>();
            }

            var matchingAttachments = conventionManager.FindMatchingAttachments(e.Instance).Select(e.Context.Resolve).Cast<IAttachment>();
            foreach (var attachment in matchingAttachments)
            {
                attachment.AttachTo(e.Instance);
            }
        }
    }
}