using Autofac;
using Caliburn.PresentationFramework.Screens;

namespace NServiceBus.Profiler.Desktop.ScreenManager
{
    public class ScreenFactory : IScreenFactory
    {
        private readonly IContainer _container;

        public ScreenFactory(IContainer container)
        {
            _container = container;
        }

        public T CreateScreen<T>() where T : IScreen
        {
            return _container.Resolve<T>();
        }
    }
}