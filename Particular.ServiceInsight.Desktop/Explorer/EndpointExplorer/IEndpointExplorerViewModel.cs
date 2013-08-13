using System.Threading.Tasks;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Caliburn.PresentationFramework.Views;
using Particular.ServiceInsight.Desktop.Events;

namespace Particular.ServiceInsight.Desktop.Explorer.EndpointExplorer
{
    public interface IEndpointExplorerViewModel :
        IExplorerViewModel,
        IScreen,
        IViewAware,
        IHandle<AutoRefreshBeat>
    {
        ServiceExplorerItem ServiceRoot { get; }
        AuditEndpointExplorerItem AuditRoot { get; }
        ErrorEndpointExplorerItem ErrorRoot { get; }
        IObservableCollection<ExplorerItem> Items { get; }
        Task ConnectToService(string serviceUrl);
    }
}