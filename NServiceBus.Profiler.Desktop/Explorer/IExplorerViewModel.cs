namespace NServiceBus.Profiler.Desktop.Explorer
{
    public interface IExplorerViewModel
    {
        int SelectedRowHandle { get; set; }
        ExplorerItem SelectedNode { get; set; }
    }
}