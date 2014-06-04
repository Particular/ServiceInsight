namespace Particular.ServiceInsight.Desktop.Core.UI.ScreenManager
{
    using Autofac;
    using Caliburn.Micro;

    public class ScreenFactory 
    {
        IContainer container;

        //TODO: SIMON remove when empty constructor fixed
        public ScreenFactory()
        {
        }

        public ScreenFactory(IContainer container)
        {
            this.container = container;
        }

        public T CreateScreen<T>() where T : IScreen
        {
            return container.Resolve<T>();
        }
    }
}