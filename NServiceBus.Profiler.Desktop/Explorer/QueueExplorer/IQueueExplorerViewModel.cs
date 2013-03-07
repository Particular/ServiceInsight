using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Caliburn.PresentationFramework.Views;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer;

namespace NServiceBus.Profiler.Desktop.Explorer.QueueExplorer
{
    public interface IQueueExplorerViewModel : 
        IExplorerViewModel,
        IScreen, 
        IViewAware,
        IQueueConnectionProvider,
        IHandle<QueueMessageCountChanged>,
        IHandle<AutoRefreshBeatEvent>
    {
        IObservableCollection<ExplorerItem> Items { get; }
        ExplorerItem MachineRoot { get; }
        ExplorerItem FolderRoot { get; }
        Queue SelectedQueue { get; }

        void ConnectToQueue(string computerName);
        void PartialRefresh();
        void FullRefresh();
        void ExpandNodes();
        void DeleteSelectedQueue();
    }
}