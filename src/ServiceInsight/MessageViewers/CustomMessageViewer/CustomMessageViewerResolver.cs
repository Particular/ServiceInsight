using Autofac;

namespace ServiceInsight.MessageViewers.CustomMessageViewer
{
    sealed class CustomMessageViewerResolver : ICustomMessageViewerResolver
    {
        private readonly ILifetimeScope autofacContainer;

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