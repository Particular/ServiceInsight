using Castle.Core.Logging;
using TestStack.White;
using TestStack.White.InputDevices;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.WindowItems;

namespace NServiceBus.Profiler.FunctionalTests.Parts
{
    public abstract class ProfilerElement
    {
        protected ProfilerElement(Window mainWindow)
        {
            MainWindow = mainWindow;
        }

        public Window MainWindow { get; private set; }
        public Application App { get; set; }
        public ILogger Logger { get; set; }
        public IKeyboard Keyboard { get; set; }
        public IMouse Mouse { get; set; }

        protected T GetByAutomationId<T>(string id)
        {
            return (T)MainWindow.Get(SearchCriteria.ByAutomationId(id));
        }

        protected T GetByClassName<T>(string className) where T : IUIItem
        {
            return (T) MainWindow.Get<T>(SearchCriteria.ByClassName(className));
        }
    }
}