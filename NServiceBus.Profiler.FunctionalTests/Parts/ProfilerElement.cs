using TestStack.White.InputDevices;
using TestStack.White.UIItems.Finders;

namespace NServiceBus.Profiler.FunctionalTests.Parts
{
    public abstract class ProfilerElement
    {
        protected ProfilerElement(IMainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }

        public IMainWindow MainWindow
        {
            get; private set;
        }

        public IKeyboard Keyboard { get; set; }
        public IMouse Mouse { get; set; }

        protected T GetByAutomationId<T>(string id)
        {
            return (T)MainWindow.Get(SearchCriteria.ByAutomationId(id));
        }
    }
}