namespace ServiceInsight.MessageViewers.CustomMessageViewer
{
    using System;
    using Anotar.Serilog;
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
            try
            {
                var viewer = autofacContainer.ResolveOptional<ICustomMessageBodyViewer>();

                if (viewer != null)
                {
                    LogTo.Information("Loaded {0} custom message viewer from the plugin.", viewer.GetType());
                }

                return viewer ?? new NopViewer();
            }
            catch (Exception ex)
            {
                LogTo.Fatal(ex, "Failed to load the custom viewer plugin.");
                return new NopViewer();
            }
        }
    }
}