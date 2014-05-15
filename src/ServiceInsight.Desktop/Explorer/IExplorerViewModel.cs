namespace Particular.ServiceInsight.Desktop.Explorer
{
    using System.Threading.Tasks;
    using Shell;

    public interface IExplorerViewModel
    {
        ExplorerItem SelectedNode { get; set; }
        void OnSelectedNodeChanged();
        Task RefreshData();
        void Navigate(string navigateUri);
        ShellViewModel Parent { get; }
    }
}