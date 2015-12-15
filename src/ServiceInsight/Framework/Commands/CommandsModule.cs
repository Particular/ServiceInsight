namespace ServiceInsight.Framework.Commands
{
    using Autofac;

    public class CommandsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CopyConversationIDCommand>().SingleInstance();
            builder.RegisterType<CopyMessageURICommand>().SingleInstance();
            builder.RegisterType<RetryMessageCommand>().SingleInstance();
            builder.RegisterType<SearchByMessageIDCommand>().SingleInstance();
            builder.RegisterType<ShowSagaCommand>().SingleInstance();
            builder.RegisterType<ChangeSelectedMessageCommand>().SingleInstance();
            builder.RegisterType<ShowExceptionCommand>().SingleInstance();
        }
    }
}