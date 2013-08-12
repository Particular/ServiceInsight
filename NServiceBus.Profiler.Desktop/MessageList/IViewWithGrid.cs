namespace NServiceBus.Profiler.Desktop.MessageList
{
    public interface IViewWithGrid
    {
        int[] GetSelectedRowsIndex();
        void BeginSelection();
        void EndSelection();
        void SelectRow(int rowIndex);
        bool IsRowSelected(int rowIndex);
    }
}