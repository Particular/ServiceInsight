using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Caliburn.PresentationFramework.Views;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Desktop.Explorer
{
    public interface IExplorerViewModel : 
        IScreen, 
        IViewAware,
        IHandle<QueueMessageCountChanged>,
        IHandle<AutoRefreshBeatEvent>
    {
        IObservableCollection<ExplorerItem> Items { get; }
        ExplorerItem MachineRoot { get; }
        ExplorerItem FolderRoot { get; }
        ExplorerItem SelectedNode { get; set; }
        Queue SelectedQueue { get; }
        string ConnectedToComputer { get; }
        int SelectedRowHandle { get; set; }
        void ConnectToQueue(string computerName);
        void ConnectToService(string serviceUrl);
        void RefreshMessageCount();
        void RefreshQueues();
        void ExpandNodes();
        void DeleteSelectedQueue();
    }
}