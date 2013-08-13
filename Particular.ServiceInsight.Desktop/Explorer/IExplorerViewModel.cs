using System;
using System.Threading.Tasks;

namespace Particular.ServiceInsight.Desktop.Explorer
{
    public interface IExplorerViewModel
    {
        int SelectedRowHandle { get; set; }
        ExplorerItem SelectedNode { get; set; }
        Task FullRefresh();
        Task PartialRefresh();
        void Navigate(string navigateUri);
    }
}