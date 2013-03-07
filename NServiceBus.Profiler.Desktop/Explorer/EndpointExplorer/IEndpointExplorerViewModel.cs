using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Caliburn.PresentationFramework.Views;
using NServiceBus.Profiler.Common.Events;

namespace NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer
{
    public interface IEndpointExplorerViewModel :
        IExplorerViewModel,
        IScreen,
        IViewAware,
        IEndpointConnectionProvider,
        IHandle<AutoRefreshBeatEvent>
    {
        ExplorerItem ServiceRoot { get; }
        ExplorerItem AuditRoot { get; }
        ExplorerItem ErrorRoot { get; }
        IObservableCollection<ExplorerItem> Items { get; }
        void ConnectToService(string serviceUrl);
        void FullRefresh();
        void PartialRefresh();
    }
}