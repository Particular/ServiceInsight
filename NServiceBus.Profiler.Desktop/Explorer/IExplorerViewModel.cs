using System.Threading.Tasks;

namespace NServiceBus.Profiler.Desktop.Explorer
{
    public interface IExplorerViewModel
    {
        ExplorerItem SelectedNode { get; set; }
        void OnSelectedNodeChanged();
        Task FullRefresh();
        Task PartialRefresh();
        void Navigate(string navigateUri);
    }
}