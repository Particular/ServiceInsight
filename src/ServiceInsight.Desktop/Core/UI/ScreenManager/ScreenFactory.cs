namespace Particular.ServiceInsight.Desktop.Core.UI.ScreenManager
{
    using Autofac;

    public class ScreenFactory : IScreenFactory
    {
        private readonly IContainer _container;

        public ScreenFactory(IContainer container)
        {
            _container = container;
        }

        public T CreateScreen<T>() where T : class
        {
            return _container.Resolve<T>();
        }
    }
}