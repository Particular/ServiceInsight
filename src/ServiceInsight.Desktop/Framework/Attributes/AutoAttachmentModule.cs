using System;
using System.Linq;
using Autofac;
using Autofac.Core;

namespace Particular.ServiceInsight.Desktop.Framework.Attributes
{
    class AutoAttachmentModule : Module
    {
        protected override void AttachToComponentRegistration(
            IComponentRegistry componentRegistry,
            IComponentRegistration registration)
        {
            registration.Activating += Activating;
        }

        private void Activating(object sender, ActivatingEventArgs<object> e)
        {
            var vmType = e.Instance.GetType();

            if (!vmType.FullName.EndsWith("ViewModel"))
                return;

            // Convention to find attachments from a ViewModel
            // This can be done better.
            var attachmentType = Type.GetType(vmType.FullName.Replace("ViewModel", "Attachment"));

            if (attachmentType == null || !e.Context.IsRegistered(attachmentType))
                return;

            var attachment = (IAttachment)e.Context.Resolve(attachmentType);

            attachment.AttachTo(e.Instance);
        }
    }
}