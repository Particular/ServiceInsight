using System.Threading.Tasks;
using NServiceBus.Profiler.Desktop.Shell;

namespace NServiceBus.Profiler.Desktop.Explorer
{
    public interface IExplorerViewModel
    {
        ExplorerItem SelectedNode { get; set; }
        void OnSelectedNodeChanged();
        Task RefreshData();
        void Navigate(string navigateUri);
        IShellViewModel Parent { get; }
    }
}