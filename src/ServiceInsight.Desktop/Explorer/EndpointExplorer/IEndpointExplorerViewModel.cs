namespace Particular.ServiceInsight.Desktop.Explorer.EndpointExplorer
{
    using System.Threading.Tasks;
    using Caliburn.PresentationFramework;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Screens;
    using Caliburn.PresentationFramework.Views;
    using Events;

    public interface IEndpointExplorerViewModel :
        IExplorerViewModel,
        IScreen,
        IViewAware,
        IHandle<RequestSelectingEndpoint>
    {
        ServiceControlExplorerItem ServiceControlRoot { get; }
        AuditEndpointExplorerItem AuditRoot { get; }
        ErrorEndpointExplorerItem ErrorRoot { get; }
        IObservableCollection<ExplorerItem> Items { get; }
        Task ConnectToService(string serviceUrl);
    }
}