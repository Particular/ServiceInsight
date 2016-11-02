namespace ServiceInsight.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Autofac;
    using Pirac;

    class AutofacContainer : Pirac.IContainer
    {
        Autofac.IContainer container;

        public void Configure(IEnumerable<Type> views, IEnumerable<Type> viewModels, IEnumerable<Type> attachments, IConventionManager conventionManager)
        {
            var builder = new ContainerBuilder();

            foreach (var type in views.Concat(viewModels).Concat(attachments))
            {
                builder.RegisterType(type);
            }

            builder.RegisterInstance(conventionManager).As<IConventionManager>().SingleInstance();

            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());

            container = builder.Build();

            // We reregister the container within itself.
            // This is bad and we should feel bad about it.
            builder = new ContainerBuilder();
            builder.RegisterInstance(container).SingleInstance();
            builder.Update(container);
        }

        public object GetInstance(Type type) => container.Resolve(type);

        public T GetInstance<T>() => container.Resolve<T>();
    }
}