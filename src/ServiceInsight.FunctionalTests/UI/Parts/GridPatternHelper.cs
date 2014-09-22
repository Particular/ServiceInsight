namespace Particular.ServiceInsight.FunctionalTests.UI.Parts
{
    using System.Windows.Automation;

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