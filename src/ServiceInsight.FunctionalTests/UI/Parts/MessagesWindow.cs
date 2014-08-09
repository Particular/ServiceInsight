namespace Particular.ServiceInsight.FunctionalTests.UI.Parts
{
    using System;
    using TestStack.White.UIItems;
    using TestStack.White.UIItems.Finders;
    using TestStack.White.UIItems.WindowItems;

    public class MessagesWindow : UIElement
    {
        public static class Columns
        {
            public static GridColumn Status;
            public static GridColumn MessageId;
            public static GridColumn MessageType;
            public static GridColumn TimeSent;
            public static GridColumn CriticalTime;
            public static GridColumn ProcessingTime;
            public static GridColumn DeliveryTime;

            static Columns()
            {
                Status = new GridColumn(0, "Status");
                MessageId = new GridColumn(1, "MessageId");
                MessageType = new GridColumn(2, "MessageType");
                TimeSent = new GridColumn(3, "TimeSent");
                CriticalTime = new GridColumn(4, "CriticalTime");
                ProcessingTime = new GridColumn(5, "ProcessingTime");
                DeliveryTime = new GridColumn(6 , "DeliveryTime");
            }
        }

        public MessagesWindow(Lazy<Window> mainWindow) : base(mainWindow)
        {
        }

        public int GetMessageCount()
        {
            var grid = GetGrid();
            return grid.Rows.Count;
        }

        public string GetCellValue(int rowIndex, GridColumn column)
        {
            var grid = GetGrid();
            var element = grid.AutomationElement;
            
            return element.GetCellValue(rowIndex, column.Index);
        }

        public void SelectRow(int rowIndex)
        {
            var grid = GetGrid();
            var row = grid.Rows[rowIndex];
            
            row.Select();
        }

        ListView GetGrid()
        {
            var barManager = GetByAutomationId<GroupBox>("BarManager");
            var grid = barManager.Get<ListView>(SearchCriteria.ByAutomationId("grid"));
            return grid;
        }
    }
}