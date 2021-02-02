namespace ServiceInsight.MessageViewers.CustomMessageViewer
{
    using Autofac;

    sealed class CustomMessageViewerResolver : ICustomMessageViewerResolver
    {
        readonly ILifetimeScope autofacContainer;

        public CustomMessageViewerResolver(ILifetimeScope autofacContainer)
        {
            this.autofacContainer = autofacContainer;
        }

        public ICustomMessageBodyViewer GetCustomMessageBodyViewer()
        {
            return autofacContainer.ResolveOptional<ICustomMessageBodyViewer>() ?? new NopViewer();
        }
    }
}