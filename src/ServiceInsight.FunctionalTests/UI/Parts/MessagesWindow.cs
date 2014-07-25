namespace Particular.ServiceInsight.FunctionalTests.UI.Parts
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Automation;
    using TestStack.White.UIItems;
    using TestStack.White.UIItems.Finders;
    using TestStack.White.UIItems.WindowItems;

    public class GridColumn
    {
        public int Index { get; private set; }
        public string Name { get; private set; }

        public GridColumn(int index, string name)
        {
            Index = index;
            Name = name;
        }
    }

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

        public MessagesWindow(Window mainWindow) : base(mainWindow)
        {
        }

        public int GetMessageCount()
        {
            var grid = GetGrid();
            return grid.Rows.Count;
        }

        public string GetCellValue(int row, GridColumn column)
        {
            var grid = GetGrid();
            var element = grid.AutomationElement;
            
            return element.GetCellValue(row, column.Index);
        }

        ListView GetGrid()
        {
            var barManager = GetByAutomationId<GroupBox>("BarManager");
            var grid = barManager.Get<ListView>(SearchCriteria.ByAutomationId("grid"));
            return grid;
        }
    }

    public static class GridPatternHelper
    {
        public static string GetCellValue(this AutomationElement grid, int row, int col)
        {
            var gridPattern = (GridPattern)grid.GetCurrentPattern(GridPattern.Pattern);
            var item = gridPattern.GetItem(row, col);
            var valuePattern = (ValuePattern)item.GetCurrentPattern(ValuePattern.Pattern);
            
            return valuePattern.Current.Value;
        }
    }

}