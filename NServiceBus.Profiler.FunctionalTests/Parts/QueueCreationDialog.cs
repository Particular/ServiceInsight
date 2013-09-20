using System.Runtime.InteropServices;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.WindowItems;

namespace NServiceBus.Profiler.FunctionalTests.Screens
{
    public class QueueCreationDialog : ProfilerElement
    {
        private Window dialog;

        public QueueCreationDialog(IMainWindow mainWindow) : base(mainWindow)
        {
        }

        public void Activate()
        {
            dialog = MainWindow.ModalWindow("Queue");
        }

        public TextBox QueueName
        {
            get { return dialog.Get<TextBox>("QueueName"); }
        }

        public Button Okay
        {
            get { return dialog.Get<Button>(SearchCriteria.ByAutomationId("OK")); }
        }

        public Button Cancel
        {
            get { return dialog.Get<Button>(SearchCriteria.ByAutomationId("Cancel")); }
        }
    }
}