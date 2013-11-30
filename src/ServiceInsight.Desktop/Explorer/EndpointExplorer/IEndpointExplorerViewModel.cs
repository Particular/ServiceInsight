using System.Threading.Tasks;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.Screens;
using Caliburn.PresentationFramework.Views;

namespace NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer
{
    public interface IEndpointExplorerViewModel :
        IExplorerViewModel,
        IScreen,
        IViewAware
    {
        ServiceControlExplorerItem ServiceControlRoot { get; }
        AuditEndpointExplorerItem AuditRoot { get; }
        ErrorEndpointExplorerItem ErrorRoot { get; }
        IObservableCollection<ExplorerItem> Items { get; }
        Task ConnectToService(string serviceUrl);
    }
}