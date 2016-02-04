namespace ServiceInsight.Framework.Commands
{
    using Autofac;

    public class CommandsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(GetType().Assembly)
                .Where(t => t.BaseType == typeof(BaseCommand))
                .AsSelf()
                .SingleInstance();
        }
    }
}