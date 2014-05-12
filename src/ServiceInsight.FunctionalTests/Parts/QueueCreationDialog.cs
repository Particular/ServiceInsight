namespace Particular.ServiceInsight.FunctionalTests.Parts
{
    using TestStack.White.UIItems;
    using TestStack.White.UIItems.Finders;
    using TestStack.White.UIItems.WindowItems;

    public class QueueCreationDialog : ProfilerElement
    {
        Window dialog;

        public QueueCreationDialog(Window mainWindow) : base(mainWindow)
        {
        }

        public void Activate()
        {
            dialog = MainWindow.ModalWindow(SearchCriteria.ByAutomationId("QueueConnectionDialog"));
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