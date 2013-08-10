using DevExpress.Xpf.Grid;

namespace NServiceBus.Profiler.Desktop.MessageList
{
    public interface IViewWithGrid
    {
        TableView Table { get; }
    }
}