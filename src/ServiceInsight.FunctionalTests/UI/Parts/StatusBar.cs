namespace Particular.ServiceInsight.FunctionalTests.UI.Parts
{
    using System.Windows.Automation;
    using TestStack.White.UIItems;
    using TestStack.White.UIItems.Finders;
    using TestStack.White.UIItems.WindowItems;
    using TestStack.White.UIItems.WindowStripControls;
    using TestStack.White.UIItems.WPFUIItems;

    public class StatusBar : UIElement
    {
        public StatusBar(Window mainWindow) : base(mainWindow)
        {
        }

        public string GetStatusMessage()
        {
            var barManager = GetByAutomationId<GroupBox>("BarManager");
            var toolStrip = barManager.Get<ToolStrip>("StatusBar");
            var statusButton = toolStrip.Get<Button>(SearchCriteria.ByAutomationId("StatusMessage"));
            var textPart = statusButton.Get<Label>(SearchCriteria.ByControlType(ControlType.Text));

            return textPart.Text;
        }

        public bool ImageShown()
        {
            var barManager = GetByAutomationId<GroupBox>("BarManager");
            var toolStrip = barManager.Get<ToolStrip>("StatusBar");
            var statusButton = toolStrip.Get<Button>(SearchCriteria.ByAutomationId("StatusMessage"));
            var imagePart = statusButton.Get<Image>(SearchCriteria.ByControlType(ControlType.Image));

            return imagePart.VisibleImage != null;            
        }
    }
}