namespace Particular.ServiceInsight.Desktop.Core.UI.ScreenManager
{
    using Autofac;

    public class ScreenFactory 
    {
        IContainer container;

        public ScreenFactory(IContainer container)
        {
            this.container = container;
        }

        public T CreateScreen<T>() where T : class
        {
            return container.Resolve<T>();
        }
    }
}