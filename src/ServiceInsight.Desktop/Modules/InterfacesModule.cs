namespace Particular.ServiceInsight.Desktop.Modules
{
    using System;
    using Autofac;
    using Castle.DynamicProxy;
    using Core.Infrastructure;
    using Startup;

    public class InterfacesModule : Module
    {
        ProxyGenerator generator;

        public InterfacesModule()
        {
            generator = new ProxyGenerator();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(GenerateInterfaceProxyForType<IEnvironment>(typeof (Environment))).As<IEnvironment>();
        }

        object GenerateInterfaceProxyForInstance<T>(object classToProxy) where T : class
        {
            return generator.CreateInterfaceProxyWithoutTarget<T>(new CallForwarderInterceptor(classToProxy));
        }

        object GenerateInterfaceProxyForType<T>(Type type) where T : class
        {
            return generator.CreateInterfaceProxyWithoutTarget<T>(new CallForwarderInterceptor(type));
        }
    }
}