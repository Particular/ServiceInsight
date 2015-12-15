namespace ServiceInsight.Framework.Modules
{
    using Autofac;
    using Startup;

    public class EnvironmentModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new EnvironmentWrapper());
        }
    }
}