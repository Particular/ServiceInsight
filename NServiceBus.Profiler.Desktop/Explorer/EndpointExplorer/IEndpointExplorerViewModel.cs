using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Caliburn.PresentationFramework.Views;
using NServiceBus.Profiler.Desktop.Events;

namespace NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer
{
    public interface IEndpointExplorerViewModel :
        IExplorerViewModel,
        IScreen,
        IViewAware,
        IHandle<AutoRefreshBeat>
    {
        ExplorerItem ServiceRoot { get; }
        ExplorerItem AuditRoot { get; }
        ExplorerItem ErrorRoot { get; }
        IObservableCollection<ExplorerItem> Items { get; }
        void ConnectToService(string serviceUrl);
    }
}