using System.Threading.Tasks;

namespace NServiceBus.Profiler.Desktop.Explorer
{
    public interface IExplorerViewModel
    {
        int SelectedRowHandle { get; set; }
        ExplorerItem SelectedNode { get; set; }
        Task FullRefresh();
        Task PartialRefresh();
    }
}