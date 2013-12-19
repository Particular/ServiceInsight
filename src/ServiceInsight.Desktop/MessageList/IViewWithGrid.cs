using System.Collections;
namespace NServiceBus.Profiler.Desktop.MessageList
{
    public interface IViewWithGrid
    {
        object ItemsSource { get; }
        object SelectedItem { get; set; }   
        int[] GetSelectedRowsIndex();
        void BeginSelection();
        void EndSelection();
        void SelectRow(int rowIndex);
        bool IsRowSelected(int rowIndex);
    }
}