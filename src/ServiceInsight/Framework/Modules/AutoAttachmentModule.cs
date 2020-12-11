using Autofac.Core.Registration;
using Autofac.Core.Resolving.Pipeline;

namespace ServiceInsight.Framework.Modules
{
    using System;
    using Attachments;
    using Autofac;
    using Autofac.Core;

    class AutoAttachmentModule : Module
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

                    var instance = c.Instance;
                    var vmType = instance?.GetType();
                    var typeName = vmType?.FullName;
                    
                    if (typeName == null || !typeName.EndsWith("ViewModel"))
                    {
                        return;
                    }

                    // Convention to find attachments from a ViewModel
                    // This can be done better.
                    var attachmentType = Type.GetType(typeName.Replace("ViewModel", "Attachment"));

                    if (attachmentType == null || !c.IsRegistered(attachmentType))
                    {
                        return;
                    }

                    var attachment = (IAttachment)c.Resolve(attachmentType);

                    attachment.AttachTo(instance);
                });
            };
        }
    }
}